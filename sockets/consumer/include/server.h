#pragma include once

#include <sys/socket.h>
#include <netinet/in.h>
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
    void Disconnect(int connection_handle);
    int GetLastErrorCode();

private:
    int max_connections_;
    unsigned short port_;
    struct sockaddr_in socketAddr_;
    int server_handle_;

    fd_set fd_set_;
    fd_set temp_fd_set_;

    int input_buffer_size_;
    unsigned char* input_buffer_;
    //unsigned integer to keep track of maximum fd value, required for select()
    unsigned short max_fd;

    void HandleNewConnection();
    void RecvFromConnection(int connection_handle);
};