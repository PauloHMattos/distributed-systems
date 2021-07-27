#include "Client.h"
#include <cstring>
#include <unistd.h>

Client::Client(int input_buffer_size)
    : input_buffer_size_(input_buffer_size)
{
    socketAddr_.sin_family = AF_INET;
    socketAddr_.sin_addr.s_addr = INADDR_ANY;
    socket_handle_ = socket(AF_INET, SOCK_STREAM, 0);
    if (socket_handle_ < 0)
    {
        perror("[CLIENT] [ERROR] Failure to create the socket");
        exit(EXIT_FAILURE);
    }

	int opt_value = 1;
    if (setsockopt(socket_handle_, SOL_SOCKET, SO_REUSEADDR, (char *) &opt_value, sizeof (int)) < 0)
    {
        perror("[CLIENT] [ERROR] Failure to set socket options");
        exit(EXIT_FAILURE);
    }
    input_buffer_ = (unsigned char*)malloc(input_buffer_size);
}

Client::~Client()
{
    Disconnect();
    free(input_buffer_);
}

int Client::GetLastErrorCode()
{
    return errno;
}

void Client::Disconnect()
{
    //m_DisconnectCallback();
    close(socket_handle_);
}

bool Client::Connect(string address, unsigned short port)
{
    if(inet_pton(AF_INET, address.c_str(), &socketAddr_.sin_addr) <= 0) 
    {
        perror("[CLIENT] [ERROR] Invalid address/ Address not supported");
        return false;
    }

    socketAddr_.sin_port = htons(port);
    if (connect(socket_handle_, (struct sockaddr *)&socketAddr_, sizeof(socketAddr_)) < 0)
    {
        perror("[CLIENT] [ERROR] Connection Failed");
        return false;
    }
    //m_ConnectedCallback(socket_handle_); //call the callback
    return true;
}

void Client::Loop()
{
    auto count = recv( socket_handle_ , input_buffer_, input_buffer_size_, 0);
    if (count <= 0)
    {
        if (count < 0)
	    {
        	perror("[CLIENT] [ERROR] recv() failed");
        	return;
        }

		Disconnect();
        return;
    }

    //m_ReceiveCallback(input_buffer);
    // clear buffer
    memset(input_buffer_, 0, input_buffer_size_);
}

int Client::Send(unsigned char* buffer, int length)
{
    int sent = send(socket_handle_, buffer, length, 0);
    if (sent < 0)
    {
        perror("[CLIENT] [ERROR] send() failed");
    }
    return sent;
}