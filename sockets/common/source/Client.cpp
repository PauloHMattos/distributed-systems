#include "Client.h"

Client::Client(short port, int input_buffer_size)
{
    peer_ = new BasePeer(input_buffer_size);
    peer_->Bind(port);
}

Client::~Client()
{
    free(peer_);
}

bool Client::Connect(string address)
{
    return peer_->Connect(address);
}

void Client::Loop()
{
    while(true)
    {
        peer_->Update();
    }
}