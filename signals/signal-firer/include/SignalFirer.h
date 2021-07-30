#pragma once

using namespace std;

class SignalFirer
{
public:
    SignalFirer(int process_number);

    void sendSignal(int signum);

private:
    int process_number_;

    bool processExists(int process_number);
};
