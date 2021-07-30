#include <iostream>
#include <cmath>
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

bool isPrime(int n);

int main(void)
{
    writer.SetBuffer(buffer, BUFFER_LENGTH);

    server.SetCallbacks(&OnRecvFromClient, &OnClientConnected, &OnClientDisconnected);

    server.Start();
    while (server.Update())
    {
    }
}

void OnRecvFromClient(SOCKET client_handle, BUFFER buffer, int length)
{
    reader.SetBuffer((unsigned char *)buffer, length);
    int32_t value = reader.ReadInt32();
    cout << "Data recevied from client [" << client_handle << "]: " << value << endl;
    if (value == 0)
    {
        server.Stop();
        exit(EXIT_SUCCESS);
    }
    bool result = isPrime(value);
    writer.Reset();
    writer.WriteBoolean(result);
    server.Send(client_handle, writer);
}

void OnClientConnected(SOCKET client_handle)
{
    cout << "Client connected: " << client_handle << endl;
}

void OnClientDisconnected(SOCKET client_handle)
{
    cout << "Client disconnected: " << client_handle << endl;
}

bool isPrime(int n)
{
    if (n <= 1)
    {
        return false;
    }
    if (n <= 3)
    {
        return true;
    }

    // This is checked so that we can skip
    // middle five numbers in below loop
    if (n % 2 == 0 || n % 3 == 0)
    {
        return false;
    }

    int sqrtN = std::sqrt(n);
    for (int i = 5; i <= sqrtN; i += 6)
    {
        if (n % i == 0 || n % (i + 2) == 0)
        {
            return false;
        }
    }
    return true;
}