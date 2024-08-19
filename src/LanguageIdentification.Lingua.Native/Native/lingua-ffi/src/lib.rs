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
    language: LinguaLanguage,
    confidence: f64,
}

pub struct LinguaPredictionListResult {
    predictions: *const LinguaPredictionResult,
    predictions_count: usize,
}

#[no_mangle]
pub unsafe extern "C" fn lingua_language_code(
    language: LinguaLanguage,
    buffer_ptr: *mut c_char
) -> size_t {
    let x: Language = language.into();
    let code = x.iso_code_639_3().to_string();
    copy_cstr(&code, buffer_ptr)
}

#[no_mangle]
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

#[no_mangle]
pub unsafe extern "C" fn lingua_language_detector_create(builder: *mut LanguageDetectorBuilder) -> *mut LanguageDetector {
    if !builder.is_null() {
        let mut builder = Box::from_raw(builder);
        let detector = builder.build();
        Box::into_raw(Box::new(detector))
    } else {
        ptr::null_mut()
    }
}

#[no_mangle]
pub unsafe extern "C" fn lingua_language_detector_builder_destroy(builder: *mut LanguageDetectorBuilder) {
    if !builder.is_null() {
        let _ = Box::from_raw(builder); // Box drops here, freeing the memory
    }
}

#[no_mangle]
pub unsafe extern "C" fn lingua_language_detector_destroy(detector: *mut LanguageDetector) {
    if !detector.is_null() {
        let _ = Box::from_raw(detector); // Box drops here, freeing the memory
    }
}

#[no_mangle]
pub unsafe extern "C" fn lingua_detect_single(
    detector: &LanguageDetector,
    text: *const c_char,
    result: *mut LinguaPredictionResult,
) -> LinguaStatus {
    let text = CStr::from_ptr(text);
    detect_single_internal(&detector, text.to_bytes(), result)
}

#[no_mangle]
pub unsafe extern "C" fn lingua_detect_multiple(
    detector: &LanguageDetector,
    text: *const c_char,
    result: *mut LinguaPredictionListResult,
) -> LinguaStatus {
    let text = CStr::from_ptr(text);
    detect_multiple_internal(&detector, text.to_bytes(), result)
}

fn detect_single_internal(
    detector: &LanguageDetector,
    text: &[u8],
    result: *mut LinguaPredictionResult,
) -> LinguaStatus {
    if result == ptr::null_mut() {
        return LinguaStatus::BadOutputPtr;
    }

    match std::str::from_utf8(text) {
        Ok(text) => {
            let res = detector.detect_language_of(text);

            match res {
                Some(value) => {
                    unsafe {
                        (*result).language = value.into();
                        (*result).confidence = detector.compute_language_confidence(text, value);
                    }
                    LinguaStatus::Ok
                }
                None => {
                    // Could not detect language
                    LinguaStatus::DetectFailure
                }
            }
        }
        Err(_) => {
            // Bad string pointer
            LinguaStatus::BadTextPtr
        }
    }
}

fn detect_multiple_internal(
    detector: &LanguageDetector,
    text: &[u8],
    result: *mut LinguaPredictionListResult,
) -> LinguaStatus {
    if result == ptr::null_mut() {
        return LinguaStatus::BadOutputPtr;
    }

    match std::str::from_utf8(text) {
        Ok(text) => {
            let predictions: Vec<LinguaPredictionResult> = detector
                .detect_multiple_languages_of(text)
                .iter()
                .map(|x| LinguaPredictionResult {
                    language: x.language().into(),
                    confidence: detector.compute_language_confidence(text, x.language())
                })
                .collect();

            let predictions_array = predictions.as_ptr();

            unsafe {
                (*result).predictions = predictions_array;
                (*result).predictions_count = predictions.len();
            }

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

    #[test]
    fn test() {
        let mut languages = vec![];

        for x in Language::all() {
            languages.push(x);
        }

        let detector: LanguageDetector = LanguageDetectorBuilder::from_languages(&languages).build();

        let text = "Привет, как дела?";

        let predictions = detector.detect_multiple_languages_of(text);

        for x in predictions {
            let language_confidence = detector.compute_language_confidence(text, x.language());
            println!("{}: {}", x.language().to_string(), language_confidence);
        }

        println!("Hello, world!");

        assert_eq!(1, 1);
    }
}
