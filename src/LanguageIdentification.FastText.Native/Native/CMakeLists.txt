project(fasttext)

cmake_minimum_required(VERSION 3.10 FATAL_ERROR)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_C_STANDARD 99)
set(CMAKE_C_STANDARD_REQUIRED ON)

if (CMAKE_CXX_COMPILER_ID MATCHES "GNU")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99 -pthread -funroll-loops -O3 -march=native")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -pthread -funroll-loops -O3 -march=native")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -static-libgcc -static-libstdc++ -fno-rtti -lpthread")
elseif (CMAKE_CXX_COMPILER_ID MATCHES "Clang")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99 -pthread -funroll-loops -O3 -march=native")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -pthread -funroll-loops -O3 -march=native")
elseif (CMAKE_CXX_COMPILER_ID MATCHES "MSVC")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /EHsc /W3 /O2 /fp:precise /arch:AVX2")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} /W3 /O2 /fp:precise /arch:AVX2")
endif()

set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

message(STATUS "CMake version: ${CMAKE_VERSION}")
message(STATUS "C++ compiler: ${CMAKE_CXX_COMPILER_ID}")
message(STATUS "C compiler: ${CMAKE_C_COMPILER_ID}")
message(STATUS "CXX flags: ${CMAKE_CXX_FLAGS}")
message(STATUS "C flags: ${CMAKE_C_FLAGS}")

add_subdirectory(fasttext EXCLUDE_FROM_ALL)

include_directories(
    ${PROJECT_SOURCE_DIR}/fasttext/src
    ${PROJECT_SOURCE_DIR}/include)


set(SOURCES ${PROJECT_SOURCE_DIR}/binding.cc)

add_library(objlib OBJECT ${SOURCES})

add_library(${PROJECT_NAME} SHARED $<TARGET_OBJECTS:objlib>)
set_target_properties(${PROJECT_NAME} PROPERTIES PUBLIC_HEADER ${PROJECT_SOURCE_DIR}/binding.h)
target_link_libraries(${PROJECT_NAME} fasttext-static_pic)

