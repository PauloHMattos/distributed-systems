# distributed-systems
Repository created to gather implementations for the Distributed Systems class (COS470 - 2021.1) from Poli-UFRJ.

## How to run the programs

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
