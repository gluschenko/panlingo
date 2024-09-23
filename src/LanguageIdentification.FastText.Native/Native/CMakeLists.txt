project(fasttext)

cmake_minimum_required(VERSION 3.10 FATAL_ERROR)

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99")
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17")
if(NOT CMAKE_BUILD_TYPE STREQUAL "Debug")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -O3")
endif()

if (CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -static-libgcc -static-libstdc++ -fno-rtti -lpthread")
elseif (CMAKE_CXX_COMPILER_ID MATCHES "MSVC")
    # nop
endif()

if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -static-libgcc -static-libstdc++ -Wl,--start-group -fpermissive -lpthread -Wl,--end-group")
    add_definitions(-D_USE_ALIGNED_MALLOC -D__USE_MINGW_ANSI_STDIO=1)
elseif(CMAKE_SYSTEM_NAME STREQUAL "Darwin")
    # nop
else()
    # nop
endif()

set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

add_subdirectory(fasttext EXCLUDE_FROM_ALL)

include_directories(
    ${PROJECT_SOURCE_DIR}/fasttext/src
    ${PROJECT_SOURCE_DIR}/include)


set(SOURCES ${PROJECT_SOURCE_DIR}/binding.cc)

add_library(objlib OBJECT ${SOURCES})

add_library(${PROJECT_NAME} SHARED $<TARGET_OBJECTS:objlib>)
set_target_properties(${PROJECT_NAME} PROPERTIES PUBLIC_HEADER ${PROJECT_SOURCE_DIR}/binding.h)
target_link_libraries(${PROJECT_NAME} fasttext-static_pic)

