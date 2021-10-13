using System;

namespace TP3.Networking
{
    public class BufferHolder
    {
        public int Position { get; protected set; }
        protected readonly byte[] _buffer;

        public BufferHolder(byte[] buffer)
        {
            _buffer = buffer;
        }

        public ReadOnlySpan<byte> AsSpan()
        {
            return _buffer.AsSpan().Slice(0, Position);
        }
    }
}