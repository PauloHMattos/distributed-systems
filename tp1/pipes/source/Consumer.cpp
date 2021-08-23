#include "Consumer.h"
#include "Utils.h"
#include <iostream>
#include <sstream>
#include <string>
#include <unistd.h>

using namespace std;

// Consumer class, which encapsulates the functionalities
// for a consumer, such as watching the pipe and printing
// the read values.
// The constructor receives an array of references to the
// pipe file descriptor.
Consumer::Consumer(int (&pipe_fd)[2])
{
    pipe_fd_ = pipe_fd;
}

int Consumer::watchPipe()
{
    char buffer[20];

    // Closes the writing edge of the pipe.
    close(*(pipe_fd_ + 1));

    while (1) {
        // Reads pipe file descriptor reading edge, saving
        // its contents to the buffer.
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

    if (Utils::isPrime(received_number)) {
        cout << "is prime.\n\n";

        return;
    }

    cout << "is not prime.\n\n";
}