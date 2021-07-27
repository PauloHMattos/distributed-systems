#pragma once

#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <string>

using namespace std;

class Client
{
public:
    Client(int input_buffer_size);
    ~Client();
    bool Connect(string address, unsigned short port);
    void Disconnect();
    void Loop();
    int GetLastErrorCode();
    int Send(unsigned char* buffer, int length);

private:
    int input_buffer_size_;
    unsigned char* input_buffer_;

    struct sockaddr_in socketAddr_;
    int socket_handle_;
};