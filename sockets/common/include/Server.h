#pragma once

#include "BasePeer.h"

class Server
{
public:
    Server(short port, int max_connections, int input_buffer_size);
    ~Server();
    bool Start();
    void Loop();
    
private:
    BasePeer *peer_;
    int max_connections_;
};