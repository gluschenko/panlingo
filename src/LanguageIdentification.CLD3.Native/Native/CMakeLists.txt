cmake_minimum_required(VERSION 3.15 FATAL_ERROR)
cmake_policy(SET CMP0091 NEW)

project(cld3)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_C_STANDARD 99)
set(CMAKE_C_STANDARD_REQUIRED ON)

add_definitions(-fPIC -D_GLIBCXX_USE_CXX11_ABI=0)

if (CMAKE_CXX_COMPILER_ID MATCHES "GNU")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99 -pthread")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -pthread -fno-rtti -lpthread -Wno-narrowing")
    set(CMAKE_C_FLAGS_RELEASE "${CMAKE_C_FLAGS_RELEASE} -funroll-loops -O3")
    set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} -funroll-loops -O3")
elseif (CMAKE_CXX_COMPILER_ID MATCHES "Clang")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99 -pthread")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -pthread -Wno-narrowing")
    set(CMAKE_C_FLAGS_RELEASE "${CMAKE_C_FLAGS_RELEASE} -funroll-loops -O3")
    set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} -funroll-loops -O3")

    if (CMAKE_OSX_ARCHITECTURES MATCHES "arm64")
        set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -arch arm64 -target arm64-apple-macos11")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -arch arm64 -target arm64-apple-macos11")
    elseif (CMAKE_OSX_ARCHITECTURES MATCHES "x86_64")
        set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -arch x86_64 -target x86_64-apple-macos10.15")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -arch x86_64 -target x86_64-apple-macos10.15")
    endif()
elseif (CMAKE_CXX_COMPILER_ID MATCHES "MSVC")
    add_definitions(-DCOMPILER_MSVC)
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /EHsc /W3 /fp:precise")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} /W3 /fp:precise")
    set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} /O2")
    set(CMAKE_C_FLAGS_RELEASE "${CMAKE_C_FLAGS_RELEASE} /O2")
    # MSVC CRT linkage
    # https://cmake.org/cmake/help/latest/variable/CMAKE_MSVC_RUNTIME_LIBRARY.html#variable:CMAKE_MSVC_RUNTIME_LIBRARY
    set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>" CACHE STRING "")
endif()

include(CheckIPOSupported)
check_ipo_supported(
    RESULT PANLINGO_IPO_SUPPORTED
    OUTPUT PANLINGO_IPO_ERROR
    LANGUAGES C CXX)
if (PANLINGO_IPO_SUPPORTED)
    set(CMAKE_INTERPROCEDURAL_OPTIMIZATION_RELEASE ON)
    message(STATUS "Release IPO/LTO: enabled")
else()
    message(STATUS "Release IPO/LTO: unavailable (${PANLINGO_IPO_ERROR})")
endif()

set(PANLINGO_RELEASE_FLAGS
    "${CMAKE_C_FLAGS} ${CMAKE_CXX_FLAGS} ${CMAKE_C_FLAGS_RELEASE} ${CMAKE_CXX_FLAGS_RELEASE} ${CMAKE_SHARED_LINKER_FLAGS} ${CMAKE_SHARED_LINKER_FLAGS_RELEASE}")
if (PANLINGO_RELEASE_FLAGS MATCHES "/Od|/RTC1|-O0|-m(arch|tune)=native|-ffast-math")
    message(FATAL_ERROR "Unsupported non-portable or non-release compiler/linker flag detected: ${PANLINGO_RELEASE_FLAGS}")
endif()

set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

message(STATUS "System name: ${CMAKE_SYSTEM_NAME}")
message(STATUS "CMake version: ${CMAKE_VERSION}")
message(STATUS "C++ compiler: ${CMAKE_CXX_COMPILER_ID}")
message(STATUS "C compiler: ${CMAKE_C_COMPILER_ID}")
message(STATUS "CXX flags (Release): ${CMAKE_CXX_FLAGS_RELEASE}")
message(STATUS "C flags (Release): ${CMAKE_C_FLAGS_RELEASE}")

add_library(${PROJECT_NAME} SHARED 
    src/base.cc
    src/embedding_feature_extractor.cc
    src/embedding_network.cc
    src/feature_extractor.cc
    src/feature_extractor.h
    src/feature_types.cc
    src/fml_parser.cc
    src/language_identifier_features.cc
    src/lang_id_nn_params.cc 
    src/nnet_language_identifier.cc
    src/registry.cc
    src/relevant_script_feature.cc
    src/sentence_features.cc
    src/task_context.cc
    src/task_context_params.cc
    src/unicodetext.cc
    src/utils.cc
    src/workspace.cc
    
    src/script_span/generated_entities.cc
    src/script_span/getonescriptspan.cc
    src/script_span/getonescriptspan.h
    src/script_span/getonescriptspan_test.cc
    src/script_span/utf8statetable.cc
    src/script_span/offsetmap.cc
    src/script_span/text_processing.cc
    src/script_span/text_processing.h
    src/script_span/fixunicodevalue.cc
    
    # bindings
    src/binding.cc
    src/binding.h
    src/fake_protobuf.h
)

set_target_properties(${PROJECT_NAME} PROPERTIES PUBLIC_HEADER "src/binding.h")

if (APPLE)
    # nop
else()
    if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
        # nop
    elseif(CMAKE_SYSTEM_NAME STREQUAL "Windows")
        target_link_libraries(${PROJECT_NAME} -static-libgcc -static-libstdc++)
    endif()
endif()

# Build unit tests
if (CMAKE_CXX_COMPILER_ID MATCHES "GNU")
    if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
        add_executable(language_identifier_main src/language_identifier_main.cc)
        target_link_libraries(language_identifier_main PRIVATE cld3)

        add_executable(getonescriptspan_test src/script_span/getonescriptspan_test.cc)
        target_link_libraries(getonescriptspan_test PRIVATE cld3)

        add_executable(language_identifier_features_test src/language_identifier_features_test.cc)
        target_link_libraries(language_identifier_features_test PRIVATE cld3)
    endif()
endif()
