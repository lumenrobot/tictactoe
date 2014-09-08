cmake_minimum_required(VERSION 2.8)
project(checkmodule)

find_package(qibuild)

# Create a executable named checkmodule
# with the source file: main.cpp
qi_create_bin(checkmodule "main.cpp")

qi_use_lib(checkmodule ALCOMMON)

