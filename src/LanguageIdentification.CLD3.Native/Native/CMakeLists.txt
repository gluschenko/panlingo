cmake_minimum_required(VERSION 3.10 FATAL_ERROR)

project(cld3)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_C_STANDARD 99)
set(CMAKE_C_STANDARD_REQUIRED ON)

if (CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang")
    # set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -static-libgcc -static-libstdc++ -fno-exceptions -fno-rtti")
elseif (CMAKE_CXX_COMPILER_ID MATCHES "MSVC")
    # nop
endif()

if(CMAKE_SYSTEM_NAME STREQUAL "Windows")
    set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -static-libgcc -static-libstdc++ -Wl,--start-group -lwinpthread -Wl,--end-group")
elseif(CMAKE_SYSTEM_NAME STREQUAL "Darwin")
    # nop
else()
    # nop
endif()

set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

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

# unit tests exec:
add_executable(language_identifier_main src/language_identifier_main.cc)
target_link_libraries(language_identifier_main PRIVATE cld3)

add_executable(getonescriptspan_test src/script_span/getonescriptspan_test.cc)
target_link_libraries(getonescriptspan_test PRIVATE cld3)

add_executable(language_identifier_features_test src/language_identifier_features_test.cc)
target_link_libraries(language_identifier_features_test PRIVATE cld3)
