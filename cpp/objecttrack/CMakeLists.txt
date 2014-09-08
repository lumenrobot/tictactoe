cmake_minimum_required(VERSION 2.8)
project(objecttrack)

find_package(qibuild)

set(_srcs
    objecttrack.cpp
    objecttrack.h
    main.cpp)

# Create a executable named objecttrack
# with the source file: main.cpp
qi_create_bin(objecttrack ${_srcs})

qi_use_lib(objecttrack ALCOMMON ALMATH)
