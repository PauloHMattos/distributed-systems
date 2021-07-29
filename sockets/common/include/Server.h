#pragma once

#include "BasePeer.h"

class Server
{
public:
    Server(short port, int max_connections, int input_buffer_size);
    ~Server();
    bool Start();
    void Loop();
    
    void SetCallbacks(void (*on_recv)(SOCKET handle, BUFFER buffer, int length),
                      void(*on_connect)(SOCKET handle),
                      void(*on_disconnect)(SOCKET handle));
    
private:
    BasePeer *peer_;
    int max_connections_;
};