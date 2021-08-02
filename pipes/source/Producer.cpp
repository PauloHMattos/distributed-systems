#include "Producer.h"
#include "Utils.h"
#include <unistd.h>
#include <iostream>
#include <string>

using namespace std;

Producer::Producer(int (&pipe_fd)[2], int number_of_numbers)
{
    pipe_fd_ = pipe_fd;
    number_of_numbers_ = number_of_numbers;
}

int Producer::produceRandomNumbers()
{
    close(*pipe_fd_);

    int number_to_be_sent = 1;

    for (int i = 0; i < number_of_numbers_; i++) {
        number_to_be_sent = number_to_be_sent + Utils::generateRandomNumber();
        string value_to_be_sent = to_string(number_to_be_sent);

        cout << "*** Producer ***\n";
        cout << "Value to be written in pipe: " << value_to_be_sent << "\n\n";

        writeToPipe(value_to_be_sent);

        sleep(1);
    }

    terminateProduction();
}

void Producer::writeToPipe(string value)
{
    write(*(pipe_fd_ + 1), value.c_str(), 21);
}

int Producer::terminateProduction()
{
    cout << "*** Producer ***\n";
    cout << "End of numbers generation." << endl;
    
    string value_to_be_sent = "0";
    
    writeToPipe(value_to_be_sent);
    
    close(*(pipe_fd_ + 1));
    
    return EXIT_SUCCESS;
}