#include "BasePeer.h"
#include <iostream>
#include <cstring>

BasePeer::BasePeer(int input_buffer_size) : input_buffer_size_(input_buffer_size)
{
    input_buffer_ = (unsigned char*)malloc(input_buffer_size);
    InitializeSocket();
}

BasePeer::~BasePeer()
{
    Close(socket_handle_);
    free(input_buffer_);

#ifdef _WIN32
    WSACleanup();
#endif
}

SOCKET BasePeer::getPeerSocket()
{
    return socket_handle_;
}

void BasePeer::InitializeSocket()
{
#if _WIN32
    WSADATA wsa_data;
    if (WSAStartup(MAKEWORD(2, 2), &wsa_data) < 0)
    {
        PrintError("Failure to initialize WinSock 2.2");
    }
#endif
    // SOCK_STREAM for TCP
    // SOCK_DGRAM for UDP
    socket_handle_ = socket(AF_INET, SOCK_STREAM, 0);

    if (socket_handle_ < 0)
    {
        PrintError("Failure to create the socket");
        exit(EXIT_FAILURE);
    }

    int opt_value = 1;
    if (setsockopt(socket_handle_, SOL_SOCKET, SO_REUSEADDR, (char *)&opt_value, sizeof(int)) < 0)
    {
        PrintError("Failure to set socket options");
        exit(EXIT_FAILURE);
    }
}

void BasePeer::Close(SOCKET handle)
{
    if (handle <= 0)
    {
        return;
    }

    on_disconnect_callback(handle);
#if _WIN32
    closesocket(handle);
#else
    shutdown(handle, 2);
#endif
    if (is_listening_)
    {
        FD_CLR(handle, &main_fds_); // remove from master set
    }
}

int BasePeer::GetLastErrorCode()
{
#ifdef _WIN32
    return WSAGetLastError();
#else
    return errno;
#endif
}

bool BasePeer::Bind(short port)
{
    memset(&socketAddr_, 0, sizeof(socketAddr_));
    socketAddr_.sin_addr.s_addr = htons(INADDR_ANY);
    socketAddr_.sin_port = htons(port);
    socketAddr_.sin_family = AF_INET;
    if (bind(socket_handle_, (struct sockaddr *)&socketAddr_, sizeof(socketAddr_)) < 0)
    {
        PrintError("Failure to bind socket");
        exit(EXIT_FAILURE);
        return false;
    }
    return true;
}

bool BasePeer::Listen(int max_connections)
{
    if (listen(socket_handle_, max_connections_) < 0)
    {
        PrintError("Listen failed");
        exit(EXIT_FAILURE);
        return false;
    }

    FD_ZERO(&main_fds_); // clear the main and temp sets
    FD_ZERO(&read_fds_);
    // main_fds_ = {0};
    FD_SET(socket_handle_, &main_fds_);
    // FD_SET(socket_handle_, &main_fds_); //insert the master socket file-descriptor into the master fd-set
    max_fds_ = socket_handle_; //set the current known maximum file descriptor count
    is_listening_ = true;
    return true;
}

SOCKET BasePeer::Connect(string remote_address, short port)
{
    is_listening_ = false;

    memset(&socketAddr_, 0, sizeof(socketAddr_));
    socketAddr_.sin_port = htons(port);
    socketAddr_.sin_family = AF_INET;
    socketAddr_.sin_addr.s_addr = inet_addr(remote_address.c_str());

    if (connect(socket_handle_, (struct sockaddr *)&socketAddr_, sizeof(socketAddr_)) < 0)
    {
        PrintError("Connection Failed");
        exit(EXIT_FAILURE);
        return -1;
    }
    on_connect_callback(socket_handle_);
    return socket_handle_;
}

int BasePeer::Send(SOCKET handle, unsigned char* buffer, int length)
{
    int total = 0;           // how many bytes we've sent
    int bytes_left = length; // how many we have left to send

    while (total < length)
    {
        int n = send(handle, (BUFFER)(buffer + total), bytes_left, 0);
        if (n == -1)
        {
            PrintError("send() failed");
            break;
        }
        total += n;
        bytes_left -= n;
    }
    return total;
}

int BasePeer::Poll()
{
    read_fds_ = main_fds_;
    return select(max_fds_ + 1, &read_fds_, NULL, NULL, NULL);
}

bool BasePeer::Update()
{
    if (is_listening_)
    {
        return UpdateServer();
    }
    return UpdateClient();
}

bool BasePeer::UpdateClient()
{
    return RecvFromConnection(socket_handle_) > 0;
}

bool BasePeer::UpdateServer()
{
    int sel = Poll();
    if (sel == -1)
    {
        PrintError("Select failed");
        return false;
    }
    //no problems, we're all set

    //loop the fd_set and check which socket has interactions available
    for (int i = 0; i <= max_fds_; i++)
    {
        if (FD_ISSET(i, &read_fds_))
        {
            //if the socket has activity pending
            if (socket_handle_ == i)
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
    return true;
}

int BasePeer::RecvFromConnection(SOCKET handle)
{
    int nbytes;
    if ((nbytes = recv(handle, (BUFFER)input_buffer_, input_buffer_size_, 0)) <= 0)
    {
        // got error or connection closed by client
        if (nbytes == 0)
        {
            // connection closed
            cout << "Connection closed" << endl;
        }
        else
        {
            PrintError("recv failed");
        }
        Close(handle); // bye!
        return -1;
    }
    on_recv_callback(handle, input_buffer_, nbytes);
    return nbytes;
}

void BasePeer::HandleNewConnection()
{
    struct sockaddr_storage client_addr;
    socklen_t addrlen = sizeof(client_addr);
    int new_connection_fd = accept(socket_handle_, (struct sockaddr *)&client_addr, &addrlen);
    if (new_connection_fd == -1)
    {
        PrintError("accept() failed");
        return;
    }

    FD_SET(new_connection_fd, &main_fds_);

    //increment the maximum known file descriptor (select() needs it)
    if (new_connection_fd > max_fds_)
    {
        max_fds_ = new_connection_fd;
    }
    on_connect_callback(new_connection_fd);
}

void BasePeer::SetCallbacks(void (*on_recv)(SOCKET handle, unsigned char* buffer, int length),
                            void (*on_connect)(SOCKET handle),
                            void (*on_disconnect)(SOCKET handle))
{
    on_recv_callback = on_recv;
    on_connect_callback = on_connect;
    on_disconnect_callback = on_disconnect;
}

void BasePeer::PrintError(const char *pcMessagePrefix)
{
    auto errorCode = GetLastErrorCode();
    cerr << "[ERROR] " << pcMessagePrefix << ": ";
    cerr << " (Id = " << errorCode << ")";
    cerr << endl;
}