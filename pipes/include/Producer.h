#pragma once
#include <string>

using namespace std;

class Producer
{
public:
    Producer(int (&pipe_fd)[2], int number_of_numbers);

    int produceRandomNumbers();

private:
    int number_of_numbers_;
    int *pipe_fd_;

    int generateRandomNumber();
    void writeToPipe(string value);
    int terminateProduction();
};