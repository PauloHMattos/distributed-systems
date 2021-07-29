#include <iostream>
#include "Server.h"

using namespace std;

int main(void) {
    Server server(7777, 1, 100);
    server.Start();
    server.Loop();
}