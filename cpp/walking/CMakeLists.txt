cmake_minimum_required(VERSION 2.8)
project(walking)

find_package(qibuild)

# Create a executable named walking
# with the source file: main.cpp
qi_create_bin(walking "main.cpp")

qi_use_lib(walking ALCOMMON)
