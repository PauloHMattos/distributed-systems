#pragma once

#include "BasePeer.h"
#include "BufferWriter.h"

class Server
{
public:
    Server(short port, int max_connections, int input_buffer_size);
    ~Server();
    bool Start();
    bool Update();
    void Stop();
    
    void SetCallbacks(void (*on_recv)(SOCKET handle, BUFFER buffer, int length),
                      void(*on_connect)(SOCKET handle),
                      void(*on_disconnect)(SOCKET handle));
    
    void Send(SOCKET handle, BufferWriter writer);
private:
    BasePeer *peer_;
    int max_connections_;
};