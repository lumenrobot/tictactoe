cmake_minimum_required(VERSION 2.8)
project(movesound)

find_package(qibuild)

option(MOVESOUND_IS_REMOTE
  "module is compiled as a remote module (ON or OFF)"
  ON)

set(_srcs
    main.cpp
    movesound.h
    movesound.cpp
)

if(MOVESOUND_IS_REMOTE)
  add_definitions(" -DMOVESOUND_IS_REMOTE ")
  qi_create_bin(movesound ${_srcs})
else()
  qi_create_lib(movesound SHARED ${_srcs} SUBFOLDER naoqi)
endif()

qi_use_lib(movesound ALCOMMON)

