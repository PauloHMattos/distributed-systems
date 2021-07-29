#pragma once

#include <cstdint>
#include "Defines.h"

class BufferReader
{
public:
    BufferReader(unsigned char* buffer, int length);
    int getPosition();

    int32_t ReadInt32();
    bool ReadBoolean();

private:
    int position_;
    unsigned char* buffer_;
    int buffer_length_;
};