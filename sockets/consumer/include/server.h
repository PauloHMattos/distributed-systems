#pragma once

#ifdef _WIN32
#include <winsock2.h>
#include <Ws2tcpip.h>
#else
#include <sys/socket.h>
#include <netinet/in.h>
typedef int SOCKET;
#endif

#include <vector>

using namespace std;

class Server
{
public:
    Server(unsigned short port, int max_connections, int input_buffer_size);
    ~Server();
    bool Bind();
    void Close();
    void Listen();
    void Loop();
    void Disconnect(SOCKET connection_handle);
    int GetLastErrorCode();
    int Send(SOCKET connection_handle, unsigned char *buffer, int length);

private:
    int max_connections_;
    unsigned short port_;
    struct sockaddr_in socketAddr_;
    SOCKET server_handle_;

    fd_set fd_set_;
    fd_set temp_fd_set_;

    int input_buffer_size_;
    unsigned char *input_buffer_;
    //unsigned integer to keep track of maximum fd value, required for select()
    unsigned short max_fd;

    void Close(SOCKET handle);
    void InitializeSocket();
    void HandleNewConnection();
    void RecvFromConnection(SOCKET connection_handle);
};