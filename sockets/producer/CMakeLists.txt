cmake_minimum_required(VERSION 3.0.0)
project(producer C CXX)

set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)

file(GLOB COMMON_FILES ../../common/source/*.cpp)
file(GLOB SOCKET_WRAPPER_FILES ../common/source/*.cpp)
file(GLOB CPP_FILES source/*.cpp)

include_directories(../common/include)
include_directories(../../common/include)

add_executable(${PROJECT_NAME} ${CPP_FILES} ${SOCKET_WRAPPER_FILES} ${COMMON_FILES})
if (WIN32)
    target_link_libraries(${PROJECT_NAME} ws2_32)
endif()