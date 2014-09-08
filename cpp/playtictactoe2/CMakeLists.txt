cmake_minimum_required(VERSION 2.8)
project(playtictactoe)

find_package(qibuild)

option(PLAYTICTACTOE_IS_REMOTE
  "module is compiled as a remote module (ON or OFF)"
  ON)

  set(_srcs
    main.cpp
    playtictactoe.h
    playtictactoe.cpp
	kinematics.h
	kinematics.cpp
)

# Create a executable named playtictactoe
# with the source file: main.cpp
if(PLAYTICTACTOE_IS_REMOTE)
  add_definitions(" -DPLAYTICTACTOE_IS_REMOTE ")
  qi_create_bin(playtictactoe ${_srcs})
else()
  qi_create_lib(playtictactoe SHARED ${_srcs} SUBFOLDER naoqi)
endif()

qi_use_lib(playtictactoe ALCOMMON ALMATH)
