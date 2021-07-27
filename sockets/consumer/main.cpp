#include <iostream>
#include "server.h"

using namespace std;

int main(void) {
    Server server(7000, 1, 100);
    server.Bind();
    server.Listen();

    while(true)
    {
        cout << "Loop" << endl;
        server.Loop();
    }
}