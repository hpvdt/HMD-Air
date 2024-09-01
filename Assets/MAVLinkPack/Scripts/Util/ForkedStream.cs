namespace MAVLinkPack.Scripts.Util
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    // TODO: use it to create multiple subscriber to a mavlink stream
    public class ForkedStream : Stream
    {
        private readonly MemoryStream _buffer;
        private readonly Stream _output1;
        private readonly Stream _output2;

        public ForkedStream(Stream output1, Stream output2)
        {
            _buffer = new MemoryStream();
            _output1 = output1;
            _output2 = output2;
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

        public override void Write(byte[] buffer, int offset, int count)
        {
            _buffer.Write(buffer, offset, count);
            _output1.Write(buffer, offset, count);
            _output2.Write(buffer, offset, count);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _buffer.WriteAsync(buffer, offset, count, cancellationToken);
            await _output1.WriteAsync(buffer, offset, count, cancellationToken);
            await _output2.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override void Flush()
        {
            _buffer.Flush();
            _output1.Flush();
            _output2.Flush();
        }

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await _buffer.FlushAsync(cancellationToken);
            await _output1.FlushAsync(cancellationToken);
            await _output2.FlushAsync(cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _buffer.Dispose();
                _output1.Dispose();
                _output2.Dispose();
            }

            base.Dispose(disposing);
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
    }
}