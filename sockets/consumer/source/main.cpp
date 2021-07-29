#include <iostream>
#include "Server.h"

using namespace std;

void OnRecvFromClient(SOCKET client_handle, BUFFER buffer, int length);
void OnClientConnected(SOCKET client_handle);
void OnClientDisconnected(SOCKET client_handle);

int main(void) {
    Server server(7000, 1, 100);

    server.SetCallbacks(&OnRecvFromClient, &OnClientConnected, &OnClientDisconnected);

    server.Start();
    server.Loop();
}

void OnRecvFromClient(SOCKET client_handle, BUFFER buffer, int length)
{
    cout << "Data recevied from client: " << client_handle << endl; 
}

void OnClientConnected(SOCKET client_handle)
{
    cout << "Client connected: " << client_handle << endl; 
}

void OnClientDisconnected(SOCKET client_handle)
{
    cout << "Client disconnected: " << client_handle << endl;
}