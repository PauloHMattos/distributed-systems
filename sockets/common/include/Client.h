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

    void SetCallbacks(void (*on_recv)(SOCKET handle, BUFFER buffer, int length),
                      void(*on_connect)(SOCKET handle),
                      void(*on_disconnect)(SOCKET handle));
                      
private:
    short port_;
    BasePeer *peer_;
};