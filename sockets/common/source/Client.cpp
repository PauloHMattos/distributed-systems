#include "Client.h"

Client::Client(short port, int input_buffer_size) : port_(port)
{
    peer_ = new BasePeer(input_buffer_size);
}

Client::~Client()
{
    free(peer_);
}

bool Client::Connect(string address)
{
    return peer_->Connect(address, port_);
}

void Client::Loop()
{
    while(true)
    {
        peer_->Update();
    }
}