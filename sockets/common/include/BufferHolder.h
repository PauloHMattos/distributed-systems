#pragma once

class BufferHolder
{
public:
    void SetBuffer(unsigned char* buffer, int length);
    void Reset();
    int getPosition();

protected:
    int position_;
    unsigned char* buffer_;
    int buffer_length_;

    void EnsureCapacity(int length);
};