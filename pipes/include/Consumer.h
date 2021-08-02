class Consumer
{
public:
    Consumer(int (&pipe_fd)[2]);

    int watchPipe();

private:
    int *pipe_fd_;

    void printStatus(int received_number);
};