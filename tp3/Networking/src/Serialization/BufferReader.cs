namespace TP3.Networking
{
    public class BufferReader : BufferHolder
    {
        public BufferReader(byte[] buffer) : base(buffer)
        {
        }

        public byte ReadByte()
        {
            return _buffer[Position++];
        }

        public int ReadInt()
        {
            int value = 0;
            value |= ReadByte();
            value |= (int)(ReadByte() << 8);
            value |= (int)(ReadByte() << 16);
            value |= (int)(ReadByte() << 24);
            return value;
        }
    }
}