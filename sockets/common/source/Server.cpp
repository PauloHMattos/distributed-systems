#include "Server.h"

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
        peer_->Update();
    }
}