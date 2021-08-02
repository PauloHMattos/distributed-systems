#include "Consumer.h"
#include "Producer.h"
#include <iostream>
#include <unistd.h>

using namespace std;

int main(void)
{
    int number_of_numbers = 2;
    int pipe_fd[2];

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

    if (pid == 0) {
        Consumer consumer(pipe_fd);

        consumer.watchPipe();
    } else {
        Producer producer(pipe_fd, number_of_numbers);

        producer.produceRandomNumbers();
    }

    return EXIT_SUCCESS;
}
