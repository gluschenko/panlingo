set(CMAKE_SYSTEM_NAME Darwin)
set(CMAKE_SYSTEM_VERSION 1)

# Path to the osxcross toolchain binaries
set(CMAKE_OSX_SYSROOT /usr/local/osxcross/target)
set(CMAKE_C_COMPILER /usr/local/osxcross/bin/o64-clang)
set(CMAKE_CXX_COMPILER /usr/local/osxcross/bin/o64-clang++)