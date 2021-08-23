#include "Producer.h"
#include "Utils.h"
#include <unistd.h>
#include <iostream>
#include <string>

using namespace std;

// Producer class which encapsulates the functionalities
// of a producer, such as produce random numbers and write
// to the pipe.
// The constructor receives an array of references to the
// pipe file descriptor.
Producer::Producer(int (&pipe_fd)[2], int number_of_numbers)
{
    pipe_fd_ = pipe_fd;
    number_of_numbers_ = number_of_numbers;
}

int Producer::produceRandomNumbers()
{
    // Closes the reading edge of the pipe.
    close(*pipe_fd_);

    // Starts with N0 = 1.
    int number_to_be_sent = 1;

    for (int i = 0; i < number_of_numbers_; i++) {
        number_to_be_sent = number_to_be_sent + Utils::generateRandomNumber();
        string value_to_be_sent = to_string(number_to_be_sent);

        cout << "*** Producer ***\n";
        cout << "Value to be written in pipe: " << value_to_be_sent << "\n\n";

        writeToPipe(value_to_be_sent);

        sleep(1);
    }

    return terminateProduction();
}

void Producer::writeToPipe(string value)
{
    // Writes writing edge, which requires the C++ string
    // to be converted to a C string.
    write(*(pipe_fd_ + 1), value.c_str(), 21);
}

int Producer::terminateProduction()
{
    cout << "*** Producer ***\n";
    cout << "End of numbers generation.\n" << endl;
    
    string value_to_be_sent = "0";
    
    writeToPipe(value_to_be_sent);
    
    close(*(pipe_fd_ + 1));
    
    return EXIT_SUCCESS;
}