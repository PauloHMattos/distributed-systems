#pragma once

#ifdef _WIN32
#include <winsock2.h>
#include <Ws2tcpip.h>
typedef char* BUFFER;
#else
#include <sys/socket.h>
#include <netinet/in.h>
typedef int SOCKET;
typedef void* BUFFER;
#endif