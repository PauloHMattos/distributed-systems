#include <iostream>
#include "Server.h"
#include "BufferWriter.h"
#include "BufferReader.h"

using namespace std;

#define BUFFER_LENGTH 100
unsigned char buffer[BUFFER_LENGTH];
BufferWriter writer;
BufferReader reader;

Server server(7000, 1, 100);
void OnRecvFromClient(SOCKET client_handle, BUFFER buffer, int length);
void OnClientConnected(SOCKET client_handle);
void OnClientDisconnected(SOCKET client_handle);

int main(void) {
    writer.SetBuffer(buffer, BUFFER_LENGTH);

    server.SetCallbacks(&OnRecvFromClient, &OnClientConnected, &OnClientDisconnected);

    server.Start();
    while(server.Update())
    {
    }
}

void OnRecvFromClient(SOCKET client_handle, BUFFER buffer, int length)
{
    reader.SetBuffer((unsigned char*)buffer, length);
    int32_t value = reader.ReadInt32();
    cout << "Data recevied from client [" << client_handle << "]: " << value << endl;
}

void OnClientConnected(SOCKET client_handle)
{
    cout << "Client connected: " << client_handle << endl;
}

void OnClientDisconnected(SOCKET client_handle)
{
    cout << "Client disconnected: " << client_handle << endl;
}