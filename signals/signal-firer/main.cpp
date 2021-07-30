#include <iostream>
#include "SignalFirer.h"

using namespace std;

int main(void) {
    int processNumber;
    int signal;

    cout << "Please type the process number you wish to send a signal to:\n";
    cin >> processNumber;

    SignalFirer firer(processNumber);

    cout << "Now please type the signal you wish to send to process\n";
    cin >> signal;

    firer.sendSignal(signal);
}
