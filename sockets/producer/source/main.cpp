#include <iostream>
#include "Client.h"

using namespace std;

void OnRecvFromServer(SOCKET client_handle, BUFFER buffer, int length);
void OnConnected(SOCKET client_handle);
void OnDisconnected(SOCKET client_handle);

int main(void)
{    
    Client client(7000, 100);
    client.SetCallbacks(&OnRecvFromServer, &OnConnected, &OnDisconnected);

    if (!client.Connect("127.0.0.1"))
    {
        exit(EXIT_FAILURE);
    }
    client.Loop();
}

void OnRecvFromServer(SOCKET client_handle, BUFFER buffer, int length)
{
    cout << "Data recevied from server" << endl; 
}

void OnConnected(SOCKET client_handle)
{
    cout << "Connection to server stablished: " << client_handle << endl; 
}

void OnDisconnected(SOCKET client_handle)
{
    cout << "Disconnected from server" << endl;
}