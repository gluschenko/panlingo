cmake_minimum_required(VERSION 2.8 FATAL_ERROR)
project(cld2_bridge)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++17")
if(NOT CMAKE_BUILD_TYPE STREQUAL "Debug")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -O3")
endif()

if(CMAKE_CXX_COMPILER_ID MATCHES "GNU|Clang")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -Wno-narrowing")
elseif(CMAKE_CXX_COMPILER_ID MATCHES "MSVC")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} /wd4244 /wd4267")
endif()


set(CMAKE_MACOSX_RPATH 1)
set(CMAKE_POSITION_INDEPENDENT_CODE ON)

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

add_library(cld2 SHARED $<TARGET_OBJECTS:objlib>)

set_target_properties(cld2 PROPERTIES PUBLIC_HEADER "${PROJECT_SOURCE_DIR}/cld2/binding.h")