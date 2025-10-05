extern crate libc;
extern crate lingua;

use libc::size_t;
use std::ffi::CStr;
use std::os::raw::c_char;
use std::ptr;
use lingua::{Language, LanguageDetector, LanguageDetectorBuilder};

#[repr(u8)]
pub enum LinguaStatus {
    Ok = 0,
    DetectFailure = 1,
    BadTextPtr = 2,
    BadOutputPtr = 3,
}

#[repr(u8)]
#[derive(Debug, Copy, Clone)]
pub enum LinguaLanguage {
    Afrikaans = 0,
    Albanian = 1,
    Arabic = 2,
    Armenian = 3,
    Azerbaijani = 4,
    Basque = 5,
    Belarusian = 6,
    Bengali = 7,
    Bokmal = 8,
    Bosnian = 9,
    Bulgarian = 10,
    Catalan = 11,
    Chinese = 12,
    Croatian = 13,
    Czech = 14,
    Danish = 15,
    Dutch = 16,
    English = 17,
    Esperanto = 18,
    Estonian = 19,
    Finnish = 20,
    French = 21,
    Ganda = 22,
    Georgian = 23,
    German = 24,
    Greek = 25,
    Gujarati = 26,
    Hebrew = 27,
    Hindi = 28,
    Hungarian = 29,
    Icelandic = 30,
    Indonesian = 31,
    Irish = 32,
    Italian = 33,
    Japanese = 34,
    Kazakh = 35,
    Korean = 36,
    Latin = 37,
    Latvian = 38,
    Lithuanian = 39,
    Macedonian = 40,
    Malay = 41,
    Maori = 42,
    Marathi = 43,
    Mongolian = 44,
    Nynorsk = 45,
    Persian = 46,
    Polish = 47,
    Portuguese = 48,
    Punjabi = 49,
    Romanian = 50,
    Russian = 51,
    Serbian = 52,
    Shona = 53,
    Slovak = 54,
    Slovene = 55,
    Somali = 56,
    Sotho = 57,
    Spanish = 58,
    Swahili = 59,
    Swedish = 60,
    Tagalog = 61,
    Tamil = 62,
    Telugu = 63,
    Thai = 64,
    Tsonga = 65,
    Tswana = 66,
    Turkish = 67,
    Ukrainian = 68,
    Urdu = 69,
    Vietnamese = 70,
    Welsh = 71,
    Xhosa = 72,
    Yoruba = 73,
    Zulu = 74,
}

#[repr(u8)]
#[derive(Debug, Copy, Clone)]
pub enum LinguaLanguageCode {
    Alpha2 = 2,
    Alpha3 = 3,
}

impl From<Language> for LinguaLanguage {
    fn from(language: Language) -> Self {
        unsafe { std::mem::transmute(language as u8) }
    }
}

impl From<LinguaLanguage> for Language {
    fn from(ffi_language: LinguaLanguage) -> Self {
        unsafe { std::mem::transmute(ffi_language as u8) }
    }
}

#[repr(C)]
#[derive(Debug)]
pub struct LinguaPredictionResult {
    pub language: LinguaLanguage,
    pub confidence: f64,
}

#[repr(C)]
#[derive(Debug)]
pub struct LinguaPredictionRangeResult {
    pub language: LinguaLanguage,
    pub confidence: f64,
    pub start_index: u32,
    pub end_index: u32,
    pub word_count: u32,
}

pub struct LinguaPredictionListResult {
    pub predictions: *const LinguaPredictionResult,
    pub predictions_count: u32,
}

pub struct LinguaPredictionRangeListResult {
    pub predictions: *const LinguaPredictionRangeResult,
    pub predictions_count: u32,
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_code(
    language: LinguaLanguage,
    code: LinguaLanguageCode,
    buffer_ptr: *mut c_char
) -> size_t {
    let x: Language = language.into();

    let code = match code {
        LinguaLanguageCode::Alpha2 => {
            x.iso_code_639_1().to_string()
        },
        LinguaLanguageCode::Alpha3 => {
            x.iso_code_639_3().to_string()
        },
    };

    copy_cstr(&code, buffer_ptr)
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_create(
    languages: *const LinguaLanguage,
    language_count: size_t
) -> *mut LanguageDetectorBuilder {
    if !languages.is_null() {
        let languages_slice = std::slice::from_raw_parts(languages, language_count);
        let languages: Vec<Language> = languages_slice.iter().map(|x| (*x).into()).collect();
        let builder = LanguageDetectorBuilder::from_languages(&languages);
        Box::into_raw(Box::new(builder))
    } else {
        ptr::null_mut()
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_with_low_accuracy_mode(
    builder: *mut LanguageDetectorBuilder
) -> *mut LanguageDetectorBuilder {
    if !builder.is_null() {
        let mut builder = Box::from_raw(builder);
        builder.with_low_accuracy_mode();
        Box::into_raw(builder)
    } else {
        ptr::null_mut()
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_with_preloaded_language_models(
    builder: *mut LanguageDetectorBuilder
) -> *mut LanguageDetectorBuilder {
    if !builder.is_null() {
        let mut builder = Box::from_raw(builder);
        builder.with_preloaded_language_models();
        Box::into_raw(builder)
    } else {
        ptr::null_mut()
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_with_minimum_relative_distance(
    builder: *mut LanguageDetectorBuilder,
    distance: f64
) -> *mut LanguageDetectorBuilder {
    if !builder.is_null() {
        let mut builder = Box::from_raw(builder);
        builder.with_minimum_relative_distance(distance);
        Box::into_raw(builder)
    } else {
        ptr::null_mut()
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_create(builder: *mut LanguageDetectorBuilder) -> *mut LanguageDetector {
    if !builder.is_null() {
        let mut builder = Box::from_raw(builder);
        let detector = builder.build();
        _ = Box::into_raw(builder);
        Box::into_raw(Box::new(detector))
    } else {
        ptr::null_mut()
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_destroy(builder: *mut LanguageDetectorBuilder) {
    if !builder.is_null() {
        let _ = Box::from_raw(builder);
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_destroy(detector: *mut LanguageDetector) {
    if !detector.is_null() {
        let _ = Box::from_raw(detector);
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_prediction_result_destroy(result: *mut LinguaPredictionResult) {
    if !result.is_null() {
        let _ = Box::from_raw(result);
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_prediction_range_result_destroy(result: *mut LinguaPredictionRangeResult) {
    if !result.is_null() {
        let _ = Box::from_raw(result);
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_detect_single(
    detector: &LanguageDetector,
    text: *const c_char,
    result: *mut LinguaPredictionListResult,
) -> LinguaStatus {
    if text.is_null() {
        return LinguaStatus::BadTextPtr;
    }

    let raw = CStr::from_ptr(text).to_bytes();
    let normalized = String::from_utf8_lossy(raw).into_owned();

    detect_single_internal(&detector, normalized.as_bytes(), result)
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_detect_mixed(
    detector: &LanguageDetector,
    text: *const c_char,
    result: *mut LinguaPredictionRangeListResult,
) -> LinguaStatus {
    if text.is_null() {
        return LinguaStatus::BadTextPtr;
    }

    let raw = CStr::from_ptr(text).to_bytes();
    let normalized = String::from_utf8_lossy(raw).into_owned();

    detect_mixed_internal(&detector, normalized.as_bytes(), result)
}

fn detect_single_internal(
    detector: &LanguageDetector,
    text: &[u8],
    result: *mut LinguaPredictionListResult,
) -> LinguaStatus {
    if result == ptr::null_mut() {
        return LinguaStatus::BadOutputPtr;
    }

    match std::str::from_utf8(text) {
        Ok(text) => {
            let predictions_vec: Vec<LinguaPredictionResult> = detector
                .compute_language_confidence_values(text)
                .iter()
                .map(|(language, value)| LinguaPredictionResult {
                    language: (*language).into(),
                    confidence: *value,
                })
                .collect();

            let predictions_count = predictions_vec.len() as u32;
            let predictions_slice = predictions_vec.into_boxed_slice();
            let predictions = predictions_slice.as_ptr();

            let new_result = LinguaPredictionListResult {
                predictions,
                predictions_count,
            };

            unsafe {
                ptr::write(result, new_result);
            }

            std::mem::forget(predictions_slice);

            LinguaStatus::Ok
        }
        Err(_) => {
            // Bad string pointer
            LinguaStatus::BadTextPtr
        }
    }
}

fn detect_mixed_internal(
    detector: &LanguageDetector,
    text: &[u8],
    result: *mut LinguaPredictionRangeListResult,
) -> LinguaStatus {
    if result == ptr::null_mut() {
        return LinguaStatus::BadOutputPtr;
    }

    match std::str::from_utf8(text) {
        Ok(text) => {
            let predictions_vec: Vec<LinguaPredictionRangeResult> = detector
                .detect_multiple_languages_of(text)
                .iter()
                .map(|x| LinguaPredictionRangeResult {
                    language: x.language().into(),
                    confidence: detector.compute_language_confidence(text, x.language()),
                    start_index: x.start_index() as u32,
                    end_index: x.end_index() as u32,
                    word_count: x.word_count() as u32,
                })
                .collect();

            let predictions_count = predictions_vec.len() as u32;
            let predictions_slice = predictions_vec.into_boxed_slice();
            let predictions = predictions_slice.as_ptr();

            let new_result = LinguaPredictionRangeListResult {
                predictions,
                predictions_count,
            };

            unsafe {
                ptr::write(result, new_result);
            }

            std::mem::forget(predictions_slice);

            LinguaStatus::Ok
        }
        Err(_) => {
            // Bad string pointer
            LinguaStatus::BadTextPtr
        }
    }
}

unsafe fn copy_cstr(src: &str, dst: *mut c_char) -> size_t {
    let len = src.len();
    if dst != ptr::null_mut() {
        let src = src.as_ptr().cast::<c_char>();
        src.copy_to_nonoverlapping(dst, len);
        *dst.add(len) = 0;
    }
    return len;
}

#[cfg(test)]
mod tests {
    use super::*;

    fn build_detector() -> LanguageDetector {
        let mut languages = vec![];

        for x in Language::all() {
            languages.push(x);
        }

        let mut builder = LanguageDetectorBuilder::from_languages(&languages);
        builder.with_preloaded_language_models();
        builder.with_minimum_relative_distance(0.9);
        builder.with_low_accuracy_mode();
        builder.build()
    }

    #[test]
    fn test() {
        let detector = build_detector();

        let text = "Привіт, як справи?";

        let language_confidence_values = detector.compute_language_confidence_values(text);
        assert_eq!(language_confidence_values[0].0, Language::Ukrainian);

        for (language, confidence) in language_confidence_values {
            println!("{}: {}", language.to_string(), confidence);
        }
    }

    #[test]
    fn lingua_invalid_utf8_fuzz() {
        let detector = build_detector();

        let raw_cases: &[&[u8]] = &[
            b"\x00",
            b"\xC2\x80",
            b"\xC2\xBF",
            b"\xED\xA0\x80", // surrogate D800
            b"\xED\xB0\x80", // surrogate DC00
            b"\xEF\xBF\xBF", // FFFF
            b"\xEF\xBF\xBE", // FFFE
            b"\xFE\xFFHello", // BOM
            b"A\xE2\x80\x8DB", // zero width joiner
            b"abc\xE2\x80\xAEdef", // RTL override
        ];

        for raw in raw_cases {
            let text = String::from_utf8_lossy(raw).to_string();

            let x = detector.compute_language_confidence_values(&text);

            for (language, confidence) in x {
                println!("{}: {}", language.to_string(), confidence);
                break;
            }
        }
    }
}
