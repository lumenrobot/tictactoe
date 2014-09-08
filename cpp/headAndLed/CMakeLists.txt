cmake_minimum_required(VERSION 2.8)
project(LumenDemo)

find_package(qibuild)

# Create a executable named LumenDemo
# with the source file: main.cpp
qi_create_bin(LumenDemo "main.cpp")

# Add a simple test:
#enable_testing()
#qi_create_test(test_LumenDemo "test.cpp")

# Tell CMake that this project depends on ALCOMMON.
# This will set the libraries to link this project with,
# the include paths, and so on
qi_use_lib(LumenDemo ALCOMMON)
