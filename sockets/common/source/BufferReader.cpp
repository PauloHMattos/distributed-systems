#include "BufferReader.h"

BufferReader::BufferReader()
{
}

void BufferReader::SetBuffer(unsigned char *buffer, int length)
{
    position_ = 0;
    buffer_ = buffer;
    buffer_length_ = length;
}

int BufferReader::getPosition()
{
    return position_;
}

int32_t BufferReader::ReadInt32()
{
    int32_t value;
    memcpy(&value, &buffer_[position_], sizeof(int32_t));
    return (int32_t)ntohl(value);
}

bool BufferReader::ReadBoolean()
{
    bool value;
    memcpy(&value, &buffer_[position_++], sizeof(bool));
    return value;
}