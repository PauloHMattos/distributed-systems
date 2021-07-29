cmake_minimum_required(VERSION 3.0.0)
project(socketswrapper VERSION 0.1.0)

set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)

if (MSYS OR MINGW)
    set(CMAKE_C_FLAGS "-static") 

    add_definitions(-DWINVER=0x0601)
    add_definitions(-D_WIN32_WINNT=0x0601)
endif()

# For shared library
add_definitions(-DSOCKETSWRAPPER_DLL)

include_directories(include)

AUX_SOURCE_DIRECTORY(source SOURCES) 
add_library(socketswrapper SHARED ${SOURCES})

if (NOT UNIX)
    target_link_libraries(socketswrapper ws2_32)
    SET_TARGET_PROPERTIES(socketswrapper PROPERTIES PREFIX "")
endif()