#pragma once

#include "Defines.h"

#include <string>

using namespace std;

class BasePeer
{
public:
    BasePeer(int input_buffer_size);
    ~BasePeer();
    SOCKET getPeerSocket();

    void InitializeSocket();
    int GetLastErrorCode();
    void Close(SOCKET handle);

    bool Bind(short port);
    bool Listen(int max_connections);
    SOCKET Connect(string remote_address, short port);

    int Send(SOCKET handle, unsigned char* buffer, int length);

    bool Update();

    void SetCallbacks(void (*on_recv)(SOCKET handle, unsigned char* buffer, int length),
                      void(*on_connect)(SOCKET handle),
                      void(*on_disconnect)(SOCKET handle));

private:
    int max_connections_;

    SOCKET socket_handle_;
    struct sockaddr_in socketAddr_;

    int input_buffer_size_;
    unsigned char* input_buffer_;

    bool is_listening_;
    
    fd_set main_fds_;
    fd_set read_fds_;

    //unsigned integer to keep track of maximum fd value, required for select()
    unsigned short max_fds_;

    
    void (*on_connect_callback) (SOCKET fd);
    void (*on_recv_callback) (SOCKET fd, unsigned char* buffer, int length);
    void (*on_disconnect_callback) (SOCKET fd);

    int Poll();
    bool UpdateServer();
    bool UpdateClient();

    int RecvFromConnection(SOCKET handle);
    void HandleNewConnection();

    void PrintError(const char* pcMessagePrefix);
};