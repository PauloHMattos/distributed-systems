#pragma once

#include <cstdint>
#include "BufferHolder.h"

class BufferReader : public BufferHolder
{
public:
    BufferReader();

    int32_t ReadInt32();
    bool ReadBoolean();
};