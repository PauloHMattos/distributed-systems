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

void Client::SetCallbacks(void (*on_recv)(SOCKET handle, BUFFER buffer, int length),
                            void (*on_connect)(SOCKET handle),
                            void (*on_disconnect)(SOCKET handle))
{
    peer_->SetCallbacks(on_recv, on_connect, on_disconnect);
}