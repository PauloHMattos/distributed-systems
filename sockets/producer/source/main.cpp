#include <iostream>
#include <random>
#include "Client.h"
#include "BufferWriter.h"
#include "BufferReader.h"

using namespace std;

#define BUFFER_LENGTH 100
unsigned char buffer[BUFFER_LENGTH];
BufferWriter writer;
BufferReader reader;

Client client(7000, 100);
void OnRecvFromServer(SOCKET client_handle, unsigned char* buffer, int length);
void OnConnected(SOCKET client_handle);
void OnDisconnected(SOCKET client_handle);

int testingNumber = 1;
int generateRandomNumber();

int main(int argc, char *argv[])
{
    if (argc < 2)
    {
        cout << "Enter the number of numbers to be tested" << endl;
        return -1;
    }

    int count = atoi(argv[1]);
    cout << count << endl;

    writer.SetBuffer(buffer, BUFFER_LENGTH);
    client.SetCallbacks(&OnRecvFromServer, &OnConnected, &OnDisconnected);

    if (!client.Connect("127.0.0.1"))
    {
        exit(EXIT_FAILURE);
    }

    int n = 1;
    do
    { 
        if (count == 0)
        {
            n = 0;
        }
        writer.Reset();
        writer.WriteInt32(n);
        client.Send(writer);
        testingNumber = n;
        n += generateRandomNumber();
        count--;
    } while(count >= 0 && client.Update());
    client.Disconnect();
}

void OnRecvFromServer(SOCKET client_handle, unsigned char* buffer, int length)
{
    reader.SetBuffer((unsigned char *)buffer, length);
    bool isPrime = reader.ReadBoolean();
    cout << testingNumber << ": ";
    if (isPrime)
    {
        cout << "Prime" << endl;
    }
    else
    {
        cout << "Not-prime" << endl;
    }
}

void OnConnected(SOCKET client_handle)
{
    cout << "Connection to server stablished: " << client_handle << endl;
}

void OnDisconnected(SOCKET client_handle)
{
    cout << "Disconnected from server" << endl;
}

int generateRandomNumber()
{
    static std::random_device rd; // obtain a random number from hardware
    static std::mt19937 gen(rd()); // seed the generator
    static std::uniform_int_distribution<> distr(1, 100); // define the range

    return distr(gen);
}