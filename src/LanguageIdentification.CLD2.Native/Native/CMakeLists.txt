cmake_minimum_required(VERSION 3.10 FATAL_ERROR)

project(cld2)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_C_STANDARD 99)
set(CMAKE_C_STANDARD_REQUIRED ON)

if (CMAKE_CXX_COMPILER_ID MATCHES "GNU")
    if(CMAKE_SYSTEM_NAME STREQUAL "Linux")
        set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99 -pthread -funroll-loops -O3")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -pthread -funroll-loops -O3")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -static-libgcc -static-libstdc++ -fno-rtti -lpthread")
    elseif(CMAKE_SYSTEM_NAME STREQUAL "Windows")
        set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -O3")
        set(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} -static-libgcc -static-libstdc++ -Wl,--start-group -lwinpthread -Wl,--end-group")
    endif()
elseif (CMAKE_CXX_COMPILER_ID MATCHES "Clang")
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -std=c99 -pthread -funroll-loops -O3")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17 -pthread -funroll-loops -O3")

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

include_directories(
    ${PROJECT_SOURCE_DIR}/cld2/internal
    ${PROJECT_SOURCE_DIR}/cld2/public
)

set(CLD2_SOURCES
    ${PROJECT_SOURCE_DIR}/cld2/internal/cldutil.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/cldutil_shared.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/compact_lang_det.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/compact_lang_det_hint_code.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/compact_lang_det_impl.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/debug.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/fixunicodevalue.cc 
    ${PROJECT_SOURCE_DIR}/cld2/internal/generated_entities.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/generated_language.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/generated_ulscript.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/getonescriptspan.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/lang_script.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/offsetmap.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/scoreonescriptspan.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/tote.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/utf8statetable.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/generated_distinct_bi_0.cc
    
    ### Chrome (less perfect predictions)
    # ${PROJECT_SOURCE_DIR}/cld2/internal/cld_generated_cjk_uni_prop_80.cc
    # ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_generated_cjk_compatible.cc
    # ${PROJECT_SOURCE_DIR}/cld2/internal/cld_generated_cjk_delta_bi_4.cc
    # ${PROJECT_SOURCE_DIR}/cld2/internal/generated_distinct_bi_0.cc
    # ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_generated_quadchrome_2.cc
    # ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_generated_deltaoctachrome.cc
    # ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_generated_distinctoctachrome.cc
    # ${PROJECT_SOURCE_DIR}/cld2/internal/cld_generated_score_quad_octa_2.cc
    ###

    ### Full
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld_generated_cjk_uni_prop_80.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_generated_cjk_compatible.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld_generated_cjk_delta_bi_32.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/generated_distinct_bi_0.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_generated_quad0122.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_generated_deltaocta0122.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_generated_distinctocta0122.cc
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld_generated_score_quad_octa_0122.cc
    ###

    ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_dynamic_compat.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_dynamic_data_extractor.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_dynamic_data.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld2_dynamic_data_loader.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/cld2tablesummary.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/cldutil.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/cldutil_offline.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/cldutil_shared.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/compact_lang_det_hint_code.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/compact_lang_det_impl.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/debug.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/fixunicodevalue.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/generated_language.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/generated_ulscript.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/getonescriptspan.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/integral_types.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/lang_script.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/langspan.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/offsetmap.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/port.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/scoreonescriptspan.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/stringpiece.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/tote.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/unittest_data.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/utf8acceptinterchange.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/utf8prop_lettermarkscriptnum.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/utf8repl_lettermarklower.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/utf8scannot_lettermarkspecial.h
    ${PROJECT_SOURCE_DIR}/cld2/internal/utf8statetable.h
    ${PROJECT_SOURCE_DIR}/cld2/public/compact_lang_det.h
    ${PROJECT_SOURCE_DIR}/cld2/public/encodings.h

    # bindings
    ${PROJECT_SOURCE_DIR}/binding.cc
    ${PROJECT_SOURCE_DIR}/binding.h
)

add_library(objlib OBJECT ${CLD2_SOURCES})

add_library(${PROJECT_NAME} SHARED $<TARGET_OBJECTS:objlib>)

set_target_properties(${PROJECT_NAME} PROPERTIES PUBLIC_HEADER "${PROJECT_SOURCE_DIR}/cld2/binding.h")

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
    target_link_libraries(${PROJECT_NAME} -static-libgcc -static-libstdc++)
endif()