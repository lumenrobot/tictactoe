cmake_minimum_required(VERSION 2.8)
project(scenetwo)

find_package(qibuild)

option(SCENETWO_IS_REMOTE
  "module is compiled as a remote module (ON or OFF)"
  ON)

set(_srcs
    main.cpp
    scenetwo.h
    scenetwo.cpp
)

if(SCENETWO_IS_REMOTE)
  add_definitions(" -DSCENETWO_IS_REMOTE ")
  qi_create_bin(scenetwo ${_srcs})
else()
  qi_create_lib(scenetwo SHARED ${_srcs} SUBFOLDER naoqi)
endif()

qi_use_lib(scenetwo ALCOMMON ALAUDIO ALMATH)
