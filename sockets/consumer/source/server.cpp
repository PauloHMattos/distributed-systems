#include "server.h"
#include <stdio.h>
#include <unistd.h>
#include <stdlib.h>
#include <errno.h>
#include <cstring>

Server::Server(unsigned short port, int max_connections, int input_buffer_size)
    : port_(port),
      max_connections_(max_connections),
      input_buffer_size_(input_buffer_size)
{
    socketAddr_.sin_family = AF_INET;
    socketAddr_.sin_addr.s_addr = INADDR_ANY;
    socketAddr_.sin_port = htons(port);

    InitializeSocket();

    input_buffer_ = (unsigned char *)malloc(input_buffer_size);
}

void Server::InitializeSocket()
{
#if _WIN32
    WSADATA wsa_data;
    if (WSAStartup(MAKEWORD(2, 2), &wsa_data) < 0)
    {
        perror("[SERVER] [ERROR] Failure to initialize WinSock");
    }
#endif
    // SOCK_STREAM for TCP
    // SOCK_DGRAM for UDP
    server_handle_ = socket(AF_INET, SOCK_STREAM, 0);

    if (server_handle_ < 0)
    {
        perror("[SERVER] [ERROR] Failure to create the socket");
        exit(EXIT_FAILURE);
    }

    int opt_value = 1;
    if (setsockopt(server_handle_, SOL_SOCKET, SO_REUSEADDR, (char *)&opt_value, sizeof(int)) < 0)
    {
        perror("[SERVER] [ERROR] Failure to set socket options");
        exit(EXIT_FAILURE);
    }
}

Server::~Server()
{
    Close();
    free(input_buffer_);

#ifdef _WIN32
    WSACleanup();
#endif
}

int Server::GetLastErrorCode()
{
#ifdef _WIN32
    return WSAGetLastError();
#else
    return errno;
#endif
}

bool Server::Bind()
{
    if (bind(server_handle_, (struct sockaddr *)&socketAddr_, sizeof(socketAddr_)) < 0)
    {
        perror("[SERVER] [ERROR] Failure to bind socket");
        exit(EXIT_FAILURE);
        return false;
    }
    
    fd_set_ = {0};
    FD_SET(server_handle_, &fd_set_); //insert the master socket file-descriptor into the master fd-set
    max_fd = server_handle_;          //set the current known maximum file descriptor count
    return true;
}

void Server::Close()
{
    for (int i = 0; i < max_fd; i++)
    {
        Disconnect(i);
    }
    Close(server_handle_);
}

void Server::Close(SOCKET handle)
{
    if (handle <= 0)
    {
        return;
    }
#if _WIN32
    closesocket(handle);
#else
    close(handle);
#endif
}

void Server::Disconnect(SOCKET connection_handle)
{
    //m_DisconnectCallback(connection_handle);
    Close(connection_handle);
}

void Server::Listen()
{
    if (listen(server_handle_, max_connections_) < 0)
    {
        perror("[SERVER] [ERROR] Listen failed");
        exit(EXIT_FAILURE);
    }
}

void Server::Loop()
{
    temp_fd_set_ = fd_set_;                                        //copy fd_set for select()
    int sel = select(max_fd + 1, &temp_fd_set_, NULL, NULL, NULL); //blocks until activity
    if (sel < 0)
    {
        printf("[SERVER] [ERROR] select() failed with error: %d\n", GetLastErrorCode());
        Close();
    }

    //no problems, we're all set

    //loop the fd_set and check which socket has interactions available
    for (int i = 0; i <= max_fd; i++)
    {
        if (FD_ISSET(i, &temp_fd_set_))
        {
            //if the socket has activity pending
            if (server_handle_ == i)
            {
                //new connection on master socket
                HandleNewConnection();
            }
            else
            {
                //exisiting connection has new data
                RecvFromConnection(i);
            }
        } //loop on to see if there is more
    }
}

void Server::HandleNewConnection()
{
    struct sockaddr_storage client_addr;
    socklen_t addrlen = sizeof(client_addr);
    int tempsocket_fd = accept(server_handle_, (struct sockaddr *)&client_addr, &addrlen);
    if (tempsocket_fd < 0)
    {
        perror("[SERVER] [ERROR] accept() failed");
        return;
    }

    FD_SET(tempsocket_fd, &fd_set_);

    //increment the maximum known file descriptor (select() needs it)
    if (tempsocket_fd > max_fd)
    {
        max_fd = tempsocket_fd;
    }
    //m_NewConnectionCallback(tempsocket_fd); //call the callback
}

void Server::RecvFromConnection(SOCKET connection_handle)
{
    int nbytesrecv = recv(connection_handle, (char *)input_buffer_, input_buffer_size_, 0);
    if (nbytesrecv <= 0)
    {
        //problem
        if (nbytesrecv < 0)
        {
            perror("[SERVER] [ERROR] recv() failed");
            return;
        }
        Disconnect(connection_handle);       //well then, bye bye.
        FD_CLR(connection_handle, &fd_set_); //clear the client fd from fd set
        return;
    }

    //m_ReceiveCallback(connectionHandle, input_buffer);

    // clear buffer
    memset(input_buffer_, 0, input_buffer_size_);
}

int Server::Send(SOCKET connection_handle, unsigned char *buffer, int length)
{
    int sent = send(connection_handle, (char *)buffer, length, 0);
    if (sent < 0)
    {
        perror("[Server] [ERROR] send() failed");
    }
    return sent;
}