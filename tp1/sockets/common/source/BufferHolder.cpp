#include "BufferHolder.h"
#include <assert.h>

void BufferHolder::SetBuffer(unsigned char *buffer, int length)
{
    position_ = 0;
    buffer_ = buffer;
    buffer_length_ = length;
}

void BufferHolder::Reset()
{
    position_ = 0;
}

int BufferHolder::getPosition()
{
    return position_;
}

void BufferHolder::EnsureCapacity(int length)
{
    assert(position_ + length <= buffer_length_);
}