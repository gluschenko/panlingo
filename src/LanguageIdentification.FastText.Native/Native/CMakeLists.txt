cmake_minimum_required(VERSION 2.8 FATAL_ERROR)
project(fasttext_bridge)

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99")
set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17")
if(NOT CMAKE_BUILD_TYPE STREQUAL "Debug")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -O3")
endif()

set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

add_subdirectory(fasttext EXCLUDE_FROM_ALL)

include_directories(
    ${PROJECT_SOURCE_DIR}/fasttext/src
    ${PROJECT_SOURCE_DIR}/include)


set(SOURCES ${PROJECT_SOURCE_DIR}/bind.cc)

add_library(objlib OBJECT ${SOURCES})

add_library(fasttext SHARED $<TARGET_OBJECTS:objlib>)
set_target_properties(fasttext PROPERTIES PUBLIC_HEADER ${PROJECT_SOURCE_DIR}/bind.h)
target_link_libraries(fasttext fasttext-static_pic)

