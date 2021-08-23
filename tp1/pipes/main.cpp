#include "Consumer.h"
#include "Producer.h"
#include <iostream>
#include <unistd.h>

using namespace std;

// Main program for executing the pipes functionality.
// It basically asks for the user input, creates the pipe
// and subsequently forks the process to establishe the
// communication between producer and consumer.
int main(void)
{
    int number_of_numbers;
    // Creates a file descriptor to store the pipe ends.
    int pipe_fd[2];

    cout << "Please type in the number of random numbers to be generated:\n";
    cin >> number_of_numbers;

    if (pipe(pipe_fd) < 0)
    {
        cout << "Error when creating the pipe. Exiting...\n";
        exit(1);
    }

    int pid = fork();

    if (pid < 0) {
        cout << "Error when creating the child process. Exiting...\n";
        exit(1);
    }

    // The child process executes the consumer whilst
    // the father plays the producer.
    if (pid == 0) {
        Consumer consumer(pipe_fd);

        consumer.watchPipe();
    } else {
        Producer producer(pipe_fd, number_of_numbers);

        producer.produceRandomNumbers();
    }

    return EXIT_SUCCESS;
}