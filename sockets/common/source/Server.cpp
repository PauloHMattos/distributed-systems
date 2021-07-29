#include "Server.h"
#include <iostream>

Server::Server(short port, int max_connections, int input_buffer_size)
{
    peer_ = new BasePeer(input_buffer_size);
    peer_->Bind(port);
    max_connections_ = max_connections;
}

Server::~Server()
{
    free(peer_);
}

bool Server::Start()
{
    return peer_->Listen(max_connections_);
}

void Server::Loop()
{
    while(true)
    {
        cout << "Loop" << endl;
        peer_->Update();
    }
}

void Server::SetCallbacks(void (*on_recv)(SOCKET handle, BUFFER buffer, int length),
                            void (*on_connect)(SOCKET handle),
                            void (*on_disconnect)(SOCKET handle))
{
    peer_->SetCallbacks(on_recv, on_connect, on_disconnect);
}