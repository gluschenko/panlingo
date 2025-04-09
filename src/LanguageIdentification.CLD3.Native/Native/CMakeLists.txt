cmake_minimum_required(VERSION 3.10 FATAL_ERROR)

project(cld3)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_C_STANDARD 99)
set(CMAKE_C_STANDARD_REQUIRED ON)

add_definitions(-fPIC -D_GLIBCXX_USE_CXX11_ABI=0)

if (CMAKE_CXX_COMPILER_ID MATCHES "GNU")
    if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
        set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99 -pthread -funroll-loops -O3")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -pthread -funroll-loops -O3")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fno-rtti -lpthread")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-narrowing")
    elseif(CMAKE_SYSTEM_NAME STREQUAL "Windows")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -O3")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -static-libgcc -static-libstdc++")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wl,--start-group -lwinpthread -Wl,--end-group")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wl,-allow-multiple-definition")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-narrowing")
    endif()
elseif (CMAKE_CXX_COMPILER_ID MATCHES "Clang")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99 -pthread -funroll-loops -O3")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -pthread -funroll-loops -O3")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-narrowing")

    if (CMAKE_OSX_ARCHITECTURES MATCHES "arm64")
        set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -arch arm64 -target arm64-apple-macos11")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -arch arm64 -target arm64-apple-macos11")
    elseif (CMAKE_OSX_ARCHITECTURES MATCHES "x86_64")
        set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -arch x86_64 -target x86_64-apple-macos10.15")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -arch x86_64 -target x86_64-apple-macos10.15")
    endif()
endif()

set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

message(STATUS "System name: ${CMAKE_SYSTEM_NAME}")
message(STATUS "CMake version: ${CMAKE_VERSION}")
message(STATUS "C++ compiler: ${CMAKE_CXX_COMPILER_ID}")
message(STATUS "C compiler: ${CMAKE_C_COMPILER_ID}")
message(STATUS "CXX flags: ${CMAKE_CXX_FLAGS}")
message(STATUS "C flags: ${CMAKE_C_FLAGS}")

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
    if (CMAKE_OSX_ARCHITECTURES MATCHES "arm64")
        set(CLANG_LIB_DIR /opt/homebrew/opt/llvm/lib/c++)
    elseif (CMAKE_OSX_ARCHITECTURES MATCHES "x86_64")
        set(CLANG_LIB_DIR /usr/local/opt/llvm/lib/c++)
    endif()

    target_link_libraries(${PROJECT_NAME}
        -nostdlib++
        -Wl,${CLANG_LIB_DIR}/libc++.a
        -Wl,${CLANG_LIB_DIR}/libc++abi.a)
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