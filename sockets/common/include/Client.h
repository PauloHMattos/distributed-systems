#pragma once

#include "BasePeer.h"
#include "BufferWriter.h"

class Client
{
public:
    Client(short port, int input_buffer_size);
    ~Client();
    bool Connect(string address);
    void Disconnect();
    bool Update();

    void SetCallbacks(void (*on_recv)(SOCKET handle, BUFFER buffer, int length),
                      void(*on_connect)(SOCKET handle),
                      void(*on_disconnect)(SOCKET handle));
           
    void Send(BufferWriter writer);
private:
    short port_;
    BasePeer *peer_;
    SOCKET socket_handle_;
};