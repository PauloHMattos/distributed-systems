#include <iostream>
#include "Client.h"

using namespace std;

int main(void)
{    
    Client client(100);
    if (!client.Connect("127.0.0.1", 7000))
    {
        exit(EXIT_FAILURE);
    }

    while(true)
    {
        cout << "Loop" << endl;
        client.Loop();
    }
}