#include <signal.h>
#include <unistd.h>
#include <iostream>

using namespace std;

int received_signum = 0;

void signalHandler(int signum)
{
    received_signum = signum;
}

void registerSignals()
{
    signal(SIGINT, signalHandler);
    signal(SIGUSR1, signalHandler);
    signal(SIGUSR2, signalHandler);
}

int listenForSignals(int execution_mode)
{
    cout << "Listening for signals SIGINT, SIGUSR1 and SIGUSR2...\n";

    while (1) {
        if (execution_mode == 2) {
            pause();
        }

        while (received_signum != 0) {

            if (received_signum == SIGINT) {
                cout << "Stopping execution...\n";

                return 0;
            } else {
                cout << "Received signal: " << received_signum << "\n";
                received_signum = 0;
            }
        }
    }
}

int main(void) {
    int execution_mode;

    cout << "Pick an execution mode:\n";
	cout << "	1 - Busy wait;\n";
	cout << "	2 - Blocking wait;\n";
    cin >> execution_mode;

    registerSignals();

    return listenForSignals(execution_mode);
}
