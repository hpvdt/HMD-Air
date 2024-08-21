namespace MAVLinkPack.Scripts.Util
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Ports;

    public class ForkingStreamWrapper : Stream
    {
        private SerialPort _serialPort;
        private List<MemoryStream> _forks = new List<MemoryStream>();
        private byte[] _buffer = new byte[4096];
        private int _readPosition = 0;
        private object _lock = new object();

        public ForkingStreamWrapper(SerialPort serialPort)
        {
            _serialPort = serialPort;
            _serialPort.DataReceived += SerialPort_DataReceived;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytesToRead = _serialPort.BytesToRead;
            byte[] receivedData = new byte[bytesToRead];
            _serialPort.Read(receivedData, 0, bytesToRead);

            lock (_lock)
            {
                foreach (var fork in _forks)
                {
                    fork.Write(receivedData, 0, bytesToRead);
                }
            }
        }

        public Stream CreateFork()
        {
            var fork = new MemoryStream();
            lock (_lock)
            {
                _forks.Add(fork);
            }

            return fork;
        }

        public void RemoveFork(Stream fork)
        {
            lock (_lock)
            {
                _forks.Remove((MemoryStream)fork);
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(
                "Reading from the wrapper stream is not supported. Use a forked stream instead.");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _serialPort.Write(buffer, offset, count);
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() => _serialPort.BaseStream.Flush();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serialPort.DataReceived -= SerialPort_DataReceived;
                foreach (var fork in _forks)
                {
                    fork.Dispose();
                }

                _forks.Clear();
            }

            base.Dispose(disposing);
        }
    }
}