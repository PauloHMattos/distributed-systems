#pragma once

#include <cstdint>
#include "BufferHolder.h"


class BufferWriter : public BufferHolder
{
public:
    BufferWriter();
    unsigned char* getBuffer();
    
    void WriteInt32(int32_t value);
    void WriteBoolean(bool value);
};