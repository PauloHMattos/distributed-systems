#include "BufferReader.h"
#include "Defines.h"
#include <cstring>

BufferReader::BufferReader()
{
}

int32_t BufferReader::ReadInt32()
{
    int len = sizeof(int32_t);
    EnsureCapacity(len);

    int32_t value;
    memcpy(&value, buffer_ + position_, len);
    position_ += len;
    return (int32_t)ntohl(value);
}

bool BufferReader::ReadBoolean()
{
    EnsureCapacity(1);

    unsigned char value = buffer_[position_];
    position_++;
    if (value == 0)
    {
        return false;
    }
    return true;
}