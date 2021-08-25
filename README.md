# distributed-systems
Repository created to gather implementations for the Distributed Systems class (COS470 - 2021.1) from Poli-UFRJ.

## How to run TP1 programs

First, enter the `tp1` folder, where the various implementations are available.

### Signals and pipes implementations:

Enter either the `signals` or `pipes` folder and run the following command:
```
make run
```

### Sockets
To build this project is required [CMake](https://cmake.org/). Open the `sockets/producer/` or `sockets/consumer/` folders, and run:
``` shell
$ mkdir build
$ cd build
build $ cmake ..
build $ make
```
The built application will be available in the path `sockets/[producer/consumer]/bin`

## How to run TP2 programs

First, enter the `tp2` folder, where the various implementations are available.

### Spinlock counter

Enter either the `spinlocks-counter` folder and run the following command:
```
dotnet run -c Release -- <#threads> <#numbers>
```

### Producer-consumer

Enter either the `producer-consumer` folder and run the following command:
```
dotnet run -c Release -- <#producers> <#consumers> <buffer_size>
```
