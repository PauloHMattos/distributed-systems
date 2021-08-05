#include "SignalFirer.h"
#include <signal.h>

// SinalFirer class, which encapsulates the functionalities
// for a signal firer, such as checking the existence of the
// process to be signaled and sending signals.
// The constructor receives the process to be signaled as an
// argument.
SignalFirer::SignalFirer(int process_number)
{
    if (!processExists(process_number)) {
        throw "Inexistent process";
    }

    process_number_ = process_number;
}

bool SignalFirer::processExists(int process_number)
{
    // Extracted from https://stackoverflow.com/a/12601815/11998835.
    return (0 == kill(process_number, 0));
}

void SignalFirer::sendSignal(int signum)
{
    kill(process_number_, signum);
}
