#include "Client.h"
#include <cstring>
#include <unistd.h>

Client::Client(int input_buffer_size)
    : input_buffer_size_(input_buffer_size)
{
    socketAddr_.sin_family = AF_INET;
    socketAddr_.sin_addr.s_addr = INADDR_ANY;
    
    InitializeSocket();
    
    input_buffer_ = (unsigned char*)malloc(input_buffer_size);
}

void Client::InitializeSocket()
{
#if _WIN32
    WSADATA wsa_data;
    if (WSAStartup(MAKEWORD(2, 2), &wsa_data) < 0)
    {
        perror("[CLIENT] [ERROR] Failure to initialize WinSock");
    }
#endif
    // SOCK_STREAM for TCP
    // SOCK_DGRAM for UDP
    socket_handle_ = socket(AF_INET, SOCK_STREAM, 0);

    if (socket_handle_ < 0)
    {
        perror("[CLIENT] [ERROR] Failure to create the socket");
        exit(EXIT_FAILURE);
    }

    int opt_value = 1;
    if (setsockopt(socket_handle_, SOL_SOCKET, SO_REUSEADDR, (char *)&opt_value, sizeof(int)) < 0)
    {
        perror("[CLIENT] [ERROR] Failure to set socket options");
        exit(EXIT_FAILURE);
    }
}

Client::~Client()
{
    Disconnect();
    free(input_buffer_);
    
#ifdef _WIN32
    WSACleanup();
#endif
}

int Client::GetLastErrorCode()
{
#ifdef _WIN32
    return WSAGetLastError();
#else
    return errno;
#endif
}

void Client::Disconnect()
{
    if (socket_handle_ <= 0)
    {
        return;
    }

    //m_DisconnectCallback();
#if _WIN32
    closesocket(socket_handle_);
#else
    close(socket_handle_);
#endif
}

bool Client::Connect(string address, const char *port)
{
    struct addrinfo hints, *res;
    memset(&hints, 0, sizeof hints);
    hints.ai_family = AF_UNSPEC;
    hints.ai_socktype = SOCK_STREAM;

    if(getaddrinfo(address.c_str(), port, &hints, &res) != 0) 
    {
        printf("[CLIENT] [ERROR] Invalid address/ Address not supported, error: %d\n", GetLastErrorCode());
        return false;
    }

    if (connect(socket_handle_, res->ai_addr, res->ai_addrlen) < 0)
    {
        printf("[CLIENT] [ERROR] Connection Failed, error: %d\n", GetLastErrorCode());
        return false;
    }
    //m_ConnectedCallback(socket_handle_); //call the callback
    return true;
}

void Client::Loop()
{
    auto count = recv( socket_handle_ , (char*)input_buffer_, input_buffer_size_, 0);
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
    int sent = send(socket_handle_, (char*)buffer, length, 0);
    if (sent < 0)
    {
        perror("[CLIENT] [ERROR] send() failed");
    }
    return sent;
}