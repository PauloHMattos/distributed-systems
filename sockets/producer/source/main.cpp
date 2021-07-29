#include <iostream>
#include "Client.h"

using namespace std;

int main(void)
{    
    Client client(7000, 100);
    if (!client.Connect("127.0.0.1"))
    {
        exit(EXIT_FAILURE);
    }
    client.Loop();
}