#pragma once

#include "BasePeer.h"

class Client
{
public:
    Client(short port, int input_buffer_size);
    ~Client();
    bool Connect(string address);
    void Disconnect();
    void Loop();

private:
    short port_;
    BasePeer *peer_;
};