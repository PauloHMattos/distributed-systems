#include "BasePeer.h"

BasePeer::BasePeer(int input_buffer_size) :
    input_buffer_size_(input_buffer_size)
{
    input_buffer_ = (BUFFER)malloc(input_buffer_size);
}

BasePeer::~BasePeer()
{
    Close(socket_handle_);
    free(input_buffer_);

#ifdef _WIN32
    WSACleanup();
#endif
}

void BasePeer::InitializeSocket()
{
#if _WIN32
    WSADATA wsa_data;
    if (WSAStartup(MAKEWORD(2, 2), &wsa_data) < 0)
    {
        perror("[ERROR] Failure to initialize WinSock 2.2");
    }
#endif
    // SOCK_STREAM for TCP
    // SOCK_DGRAM for UDP
    socket_handle_ = socket(AF_INET, SOCK_STREAM, 0);

    if (socket_handle_ < 0)
    {
        perror("[ERROR] Failure to create the socket");
        exit(EXIT_FAILURE);
    }

    int opt_value = 1;
    if (setsockopt(socket_handle_, SOL_SOCKET, SO_REUSEADDR, (char *)&opt_value, sizeof(int)) < 0)
    {
        perror("[ERROR] Failure to set socket options");
        exit(EXIT_FAILURE);
    }
}

void BasePeer::Close(SOCKET handle)
{
    if (handle <= 0)
    {
        return;
    }

    //m_ConnectionClosedCallback();
#if _WIN32
    closesocket(handle);
#else
    close(handle);
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
    socketAddr_.sin_port = htons(port);
    socketAddr_.sin_family = AF_INET;
    if (bind(socket_handle_, (struct sockaddr *)&socketAddr_, sizeof(socketAddr_)) < 0)
    {
        perror("[SERVER] [ERROR] Failure to bind socket");
        exit(EXIT_FAILURE);
        return false;
    }
    return true;
}

bool BasePeer::Listen(int max_connections)
{
    socketAddr_.sin_addr.s_addr = INADDR_ANY;
    if (listen(socket_handle_, max_connections_) < 0)
    {
        perror("[SERVER] [ERROR] Listen failed");
        exit(EXIT_FAILURE);
        return false;
    }

    FD_ZERO(&main_fds_); // clear the main and temp sets
    FD_ZERO(&read_fds_);
    // main_fds_ = {0};
    // FD_SET(socket_handle_, &main_fds_); //insert the master socket file-descriptor into the master fd-set
    max_fds_ = socket_handle_; //set the current known maximum file descriptor count
    is_listening_ = true;
    return true;
}

bool BasePeer::Connect(string remote_address)
{
    is_listening_ = false;
    socketAddr_.sin_addr.s_addr = inet_addr(remote_address.c_str());

    if (connect(socket_handle_, (struct sockaddr *)&socketAddr_, sizeof(socketAddr_)) < 0)
    {
        printf("[ERROR] Connection Failed, error: %d\n", GetLastErrorCode());
        exit(EXIT_FAILURE);
        return false;
    }
    //m_ConnectedCallback(socket_handle_);
    return true;
}

int BasePeer::Send(SOCKET handle, BUFFER buffer, int length)
{
    int sent = send(handle, (char *)buffer, length, 0);
    if (sent < 0)
    {
        perror("[ERROR] send() failed");
    }
    return sent;
}

int BasePeer::Poll()
{
    read_fds_ = main_fds_;
    return select(max_fds_ + 1, &read_fds_, NULL, NULL, NULL);
}

void BasePeer::Update()
{
    if (is_listening_)
    {
        UpdateServer();
        return;
    }
    UpdateClient();
}

void BasePeer::UpdateClient()
{
    RecvFromConnection(socket_handle_);
}

void BasePeer::UpdateServer()
{
    int sel = Poll();
    if (sel == -1)
    {
        perror("select");
        exit(EXIT_FAILURE);
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
}

int BasePeer::RecvFromConnection(SOCKET handle)
{
    int nbytes;
    if ((nbytes = recv(handle, input_buffer_, input_buffer_size_, 0)) <= 0)
    {
        // got error or connection closed by client
        if (nbytes == 0)
        {
            // connection closed
            printf("selectserver: socket %d hung up\n", handle);
        }
        else
        {
            perror("recv");
        }
        Close(handle);           // bye!
        return -1;
    }
    // Call callback
    return nbytes;
}

void BasePeer::HandleNewConnection()
{
    struct sockaddr_storage client_addr;
    socklen_t addrlen = sizeof(client_addr);
    int new_connection_fd = accept(socket_handle_, (struct sockaddr *)&client_addr, &addrlen);
    if (new_connection_fd == -1)
    {
        perror("[ERROR] accept() failed");
        return;
    }

    FD_SET(new_connection_fd, &main_fds_);

    //increment the maximum known file descriptor (select() needs it)
    if (new_connection_fd > max_fds_)
    {
        max_fds_ = new_connection_fd;
    }
    //m_NewConnectionCallback(tempsocket_fd); //call the callback
}