#pragma once
#ifdef _WIN32
#ifndef _WIN32_WINNT
#define _WIN32_WINNT 0x0501 /* Windows XP. */
#endif
#include <winsock2.h>
#include <Ws2tcpip.h>
#else
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
typedef int SOCKET;
#endif

#include <string>

using namespace std;

class Client
{
public:
    Client(int input_buffer_size);
    ~Client();
    bool Connect(string address, const char *port);
    void Disconnect();
    void Loop();
    int GetLastErrorCode();
    int Send(unsigned char *buffer, int length);

private:
    int input_buffer_size_;
    unsigned char *input_buffer_;

    struct sockaddr_in socketAddr_;
    SOCKET socket_handle_;

    void InitializeSocket();
};