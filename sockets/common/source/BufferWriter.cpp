#include "BufferWriter.h"
#include "Defines.h"
#include <cstring>

BufferWriter::BufferWriter()
{
}

unsigned char* BufferWriter::getBuffer()
{
    return buffer_;
}

void BufferWriter::WriteInt32(int32_t value)
{
    int len = sizeof(int32_t);
    EnsureCapacity(len);

    value = htonl(value);
    memcpy(&buffer_[position_], &value, len);
    position_+=len;
}

void BufferWriter::WriteBoolean(bool value)
{
    EnsureCapacity(1);
    if (value)
    {
        buffer_[position_++] = 255;
    }
    else
    {
        buffer_[position_++] = 0;
    }
}