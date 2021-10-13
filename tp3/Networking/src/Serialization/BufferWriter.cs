namespace TP3.Networking
{
    public class BufferWriter : BufferHolder
    {
        public BufferWriter(byte[] buffer) : base(buffer)
        {
        }

        public void WriteByte(byte value)
        {
            _buffer[Position++] = value;
        }

        public void WriteInt(int value)
        {
            WriteByte((byte)value);
            WriteByte((byte)(value >> 8));
            WriteByte((byte)(value >> 16));
            WriteByte((byte)(value >> 24));
        }
    }
}