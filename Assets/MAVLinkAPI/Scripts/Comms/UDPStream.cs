using System;
using System.IO;

namespace MAVLinkAPI.Scripts.Comms
{
    public class UDPStream : Stream
    {
        private UdpSerial udpSerial;

        public UDPStream(UdpSerial udpSerial)
        {
            this.udpSerial = udpSerial;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotImplementedException();

        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return udpSerial.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            udpSerial.Write(buffer, offset, count);
        }
    }
}