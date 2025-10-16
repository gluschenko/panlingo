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
) -> size_t { unsafe {
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
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_create(
    languages: *const LinguaLanguage,
    language_count: size_t
) -> *mut LanguageDetectorBuilder { unsafe {
    if !languages.is_null() {
        let languages_slice = std::slice::from_raw_parts(languages, language_count);
        let languages: Vec<Language> = languages_slice.iter().map(|x| (*x).into()).collect();
        let builder = LanguageDetectorBuilder::from_languages(&languages);
        Box::into_raw(Box::new(builder))
    } else {
        ptr::null_mut()
    }
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_with_low_accuracy_mode(
    builder: *mut LanguageDetectorBuilder
) -> *mut LanguageDetectorBuilder { unsafe {
    if !builder.is_null() {
        let mut builder = Box::from_raw(builder);
        builder.with_low_accuracy_mode();
        Box::into_raw(builder)
    } else {
        ptr::null_mut()
    }
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_with_preloaded_language_models(
    builder: *mut LanguageDetectorBuilder
) -> *mut LanguageDetectorBuilder { unsafe {
    if !builder.is_null() {
        let mut builder = Box::from_raw(builder);
        builder.with_preloaded_language_models();
        Box::into_raw(builder)
    } else {
        ptr::null_mut()
    }
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_with_minimum_relative_distance(
    builder: *mut LanguageDetectorBuilder,
    distance: f64
) -> *mut LanguageDetectorBuilder { unsafe {
    if !builder.is_null() {
        let mut builder = Box::from_raw(builder);
        builder.with_minimum_relative_distance(distance);
        Box::into_raw(builder)
    } else {
        ptr::null_mut()
    }
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_create(builder: *mut LanguageDetectorBuilder) -> *mut LanguageDetector { unsafe {
    if !builder.is_null() {
        let mut builder = Box::from_raw(builder);
        let detector = builder.build();
        _ = Box::into_raw(builder);
        Box::into_raw(Box::new(detector))
    } else {
        ptr::null_mut()
    }
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_destroy(builder: *mut LanguageDetectorBuilder) { unsafe {
    if !builder.is_null() {
        let _ = Box::from_raw(builder);
    }
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_destroy(detector: *mut LanguageDetector) { unsafe {
    if !detector.is_null() {
        let _ = Box::from_raw(detector);
    }
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_prediction_result_destroy(result: *mut LinguaPredictionResult, count: u32) {
    unsafe {
        if result.is_null() {
            return;
        }

        let count = count as usize;
        if count == 0 {
            return;
        }

        let _ = Vec::from_raw_parts(
            result,
            count,
            count,
        );
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_prediction_range_result_destroy(result: *mut LinguaPredictionRangeResult, count: u32) {
    unsafe {
        if result.is_null() {
            return;
        }

        let count = count as usize;
        if count == 0 {
            return;
        }

        let _ = Vec::from_raw_parts(
            result,
            count,
            count,
        );
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_detect_single(
    detector: &LanguageDetector,
    text: *const c_char,
    result: *mut LinguaPredictionListResult,
) -> LinguaStatus { unsafe {
    let text = CStr::from_ptr(text);
    detect_single_internal(&detector, text.to_bytes(), result)
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_detect_mixed(
    detector: &LanguageDetector,
    text: *const c_char,
    result: *mut LinguaPredictionRangeListResult,
) -> LinguaStatus { unsafe {
    let text = CStr::from_ptr(text);
    detect_mixed_internal(&detector, text.to_bytes(), result)
}}

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

unsafe fn copy_cstr(src: &str, dst: *mut c_char) -> size_t { unsafe {
    let len = src.len();
    if dst != ptr::null_mut() {
        let src = src.as_ptr().cast::<c_char>();
        src.copy_to_nonoverlapping(dst, len);
        *dst.add(len) = 0;
    }
    return len;
}}

#[cfg(test)]
mod tests {
    use super::*;
    use std::ffi::CString;

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

    fn create_detector_ptr() -> *mut LanguageDetector {
        let languages: Vec<LinguaLanguage> = Language::all()
            .into_iter()
            .map(|x| x.into())
            .collect();

        unsafe {
            let builder = lingua_language_detector_builder_create(languages.as_ptr(), languages.len());
            let builder = lingua_language_detector_builder_with_preloaded_language_models(builder);
            let builder = lingua_language_detector_builder_with_low_accuracy_mode(builder);
            let builder = lingua_language_detector_builder_with_minimum_relative_distance(builder, 0.9);
            lingua_language_detector_create(builder)
        }
    }

    #[test]
    fn test_lingua_detect_mixed() {
        unsafe {
            let detector = create_detector_ptr();
            assert!(!detector.is_null(), "detector should not be null");

            let text = CString::new("Hello world").unwrap();

            let mut result = std::mem::MaybeUninit::<LinguaPredictionRangeListResult>::uninit();

            let status = lingua_detect_mixed(detector.as_ref().unwrap(), text.as_ptr(), result.as_mut_ptr());

            assert_eq!(status as u8, LinguaStatus::Ok as u8, "status should be Ok");

            let result = result.assume_init();
            assert_eq!(result.predictions_count, 0, "there should be at least one prediction");
            assert!(!result.predictions.is_null(), "predictions pointer should not be null");

            /*if(!result.predictions.is_null()){
                let first = &*result.predictions;
                println!(
                    "Detected language: {:?}, confidence: {:.3}",
                    first.language, first.confidence
                );
            }*/

            lingua_prediction_range_result_destroy(result.predictions as *mut LinguaPredictionRangeResult, result.predictions_count);
            lingua_language_detector_destroy(detector);
            assert!(true)
        }
    }
}
