cmake_minimum_required(VERSION 3.0.0)
project(consumerProj VERSION 0.1.0)

set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${PROJECT_SOURCE_DIR}/bin)

include_directories(include)
AUX_SOURCE_DIRECTORY(source SOURCES) 

link_directories(../common/bin)
set(WRAPPER_LIB socketswrapper${CMAKE_SHARED_LIBRARY_SUFFIX})

add_executable(consumer ${SOURCES})
target_link_libraries(consumer PRIVATE ${WRAPPER_LIB})
target_include_directories(consumer PRIVATE ../common/include)