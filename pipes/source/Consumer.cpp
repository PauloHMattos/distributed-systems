#include "Consumer.h"
#include <iostream>
#include <sstream>
#include <string>
#include <unistd.h>
#include <cmath>

using namespace std;

Consumer::Consumer(int (&pipe_fd)[2])
{
    pipe_fd_ = pipe_fd;
}

int Consumer::watchPipe()
{
    char buffer[20];

    close(*(pipe_fd_ + 1));

    while (1) {
        read(*pipe_fd_, &buffer, 21);
        const string received_char = buffer;
        int received_number = stoi(received_char);

        if (received_char == "0") {
            cout << "*** Consumer ***\n";
            cout << "0 received. Terminating excution.\n";

            break;
        }

        printStatus(received_number);
    }

    return EXIT_SUCCESS;
}

void Consumer::printStatus(int received_number)
{
    cout << "*** Consumer ***\n";
    cout << "Received number " << received_number << ", which ";

    if (isPrime(received_number)) {
        cout << "is prime.\n\n";

        return;
    }

    cout << "is not prime.\n\n";
}

bool Consumer::isPrime(int n)
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