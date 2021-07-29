#include "BufferWritter.h"

BufferWritter::BufferWritter(unsigned char *buffer, int length) :
    position_(0),
    buffer_(buffer),
    buffer_length_(length)
{
}

int BufferWritter::getPosition()
{
    return position_;
}

void BufferWritter::WriteInt32(int32_t value)
{
    value = htonl(value);
    memcpy(&buffer_[position_], &value, sizeof(int32_t));
    position_++;
}

void BufferWritter::WriteBoolean(bool value)
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