#include "BufferWriter.h"

BufferWriter::BufferWriter()
{
}

void BufferWriter::SetBuffer(unsigned char *buffer, int length)
{
    position_ = 0;
    buffer_ = buffer;
    buffer_length_ = length;
}


void BufferWriter::Reset()
{
    position_ = 0;
}

int BufferWriter::getPosition()
{
    return position_;
}

unsigned char* BufferWriter::getBuffer()
{
    return buffer_;
}

void BufferWriter::WriteInt32(int32_t value)
{
    value = htonl(value);
    memcpy(&buffer_[position_], &value, sizeof(int32_t));
    position_++;
}

void BufferWriter::WriteBoolean(bool value)
{
    if (value)
    {
        buffer_[position_++] = 255;
    }
    else
    {
        buffer_[position_++] = 0;
    }
}