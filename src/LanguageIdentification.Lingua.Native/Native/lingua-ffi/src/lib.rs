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

#[repr(C)]
#[derive(Debug)]
pub struct DetectionResult {
    language: Language,
    confidence: f64,
}

pub struct LanguageDetectorListResult {
    predictions: [DetectionResult],
    predictions_count: usize,
}

#[no_mangle]
pub unsafe extern "C" fn lingua_language_code(language: Language, buffer_ptr: *mut c_char) -> size_t {
    let code = language.iso_code_639_3().to_string();
    copy_cstr(&code, buffer_ptr)
}

#[no_mangle]
pub unsafe extern "C" fn lingua_language_detector_builder_create(languages: *const Language, language_count: size_t) -> *mut LanguageDetectorBuilder {
    if !languages.is_null() {
        let languages_slice = std::slice::from_raw_parts(languages, language_count);
        let builder = LanguageDetectorBuilder::from_languages(&languages_slice);
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
    result: *mut DetectionResult,
) -> LinguaStatus {
    let text = CStr::from_ptr(text);
    detect_single_internal(&detector, text.to_bytes(), result)
}

#[no_mangle]
pub unsafe extern "C" fn lingua_detect_multiple(
    detector: &LanguageDetector,
    text: *const c_char,
    result: *mut DetectionResult,
) -> LinguaStatus {
    let text = CStr::from_ptr(text);
    detect_multiple_internal(&detector, text.to_bytes(), result)
}

fn detect_single_internal(
    detector: &LanguageDetector,
    text: &[u8],
    result: *mut DetectionResult,
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
                        (*result).language = value;
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
    result: *mut LanguageDetectorListResult,
) -> LinguaStatus {
    if result == ptr::null_mut() {
        return LinguaStatus::BadOutputPtr;
    }

    match std::str::from_utf8(text) {
        Ok(text) => {
            let res = detector.detect_multiple_languages_of(text).iter().collect();

            unsafe {
                (*result).predictions = res;
                (*result).predictions_count = res.len();
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
