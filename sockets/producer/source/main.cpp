#include <iostream>
#include "Client.h"
#include "BufferWriter.h"
#include "BufferReader.h"

using namespace std;

#define BUFFER_LENGTH 100
unsigned char buffer[BUFFER_LENGTH];
BufferWriter writer;
BufferReader reader;

Client client(7000, 100);
void OnRecvFromServer(SOCKET client_handle, BUFFER buffer, int length);
void OnConnected(SOCKET client_handle);
void OnDisconnected(SOCKET client_handle);


int main(void)
{
    writer.SetBuffer(buffer, BUFFER_LENGTH);
    client.SetCallbacks(&OnRecvFromServer, &OnConnected, &OnDisconnected);

    if (!client.Connect("127.0.0.1"))
    {
        exit(EXIT_FAILURE);
    }

    do
    {
        writer.Reset();
        writer.WriteInt32(100);
        client.Send(writer);
    } while(client.Update());
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