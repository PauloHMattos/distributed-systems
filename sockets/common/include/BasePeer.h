#pragma once

#include "Defines.h"

#include <string>

using namespace std;

class BasePeer
{
public:
    BasePeer(int input_buffer_size);
    ~BasePeer();

    void InitializeSocket();
    int GetLastErrorCode();
    void Close(SOCKET handle);

    bool Bind(short port);
    bool Listen(int max_connections);
    bool Connect(string remote_address, short port);

    int Send(SOCKET handle, BUFFER buffer, int length);

    void Update();

    void SetCallbacks();

private:
    int max_connections_;

    SOCKET socket_handle_;
    struct sockaddr_in socketAddr_;

    int input_buffer_size_;
    BUFFER input_buffer_;

    bool is_listening_;
    
    fd_set main_fds_;
    fd_set read_fds_;

    //unsigned integer to keep track of maximum fd value, required for select()
    unsigned short max_fds_;

    int Poll();
    void UpdateServer();
    void UpdateClient();

    int RecvFromConnection(SOCKET handle);
    void HandleNewConnection();

    void PrintError(const char* pcMessagePrefix);
};