#pragma once

#include <cstdint>
#include "Defines.h"

class BufferWritter
{
public:
    BufferWritter(unsigned char* buffer, int length);
    int getPosition();

    void WriteInt32(int32_t value);
    void WriteBoolean(bool value);

private:
    int position_;
    unsigned char* buffer_;
    int buffer_length_;
};