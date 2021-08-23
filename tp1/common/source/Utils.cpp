#include "Utils.h"
#include <cmath>
#include <random>

int Utils::generateRandomNumber()
{
    static std::random_device rd; // obtain a random number from hardware
    static std::mt19937 gen(rd()); // seed the generator
    static std::uniform_int_distribution<> distr(1, 100); // define the range

    return distr(gen);
}

bool Utils::isPrime(int n)
{
    if (n <= 1)
    {
        return false;
    }
    if (n <= 3)
    {
        return true;
    }

    // This is checked so that we can skip
    // middle five numbers in below loop
    if (n % 2 == 0 || n % 3 == 0)
    {
        return false;
    }

    int sqrtN = std::sqrt(n);
    for (int i = 5; i <= sqrtN; i += 6)
    {
        if (n % i == 0 || n % (i + 2) == 0)
        {
            return false;
        }
    }
    return true;
}