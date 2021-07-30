#pragma once

#ifdef _WIN32
#include <winsock2.h>
#include <Ws2tcpip.h>
typedef char* BUFFER;
#else
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
typedef int SOCKET;
typedef void* BUFFER;
#ifdef __APPLE__
#include <sys/select.h>
#endif
#endif