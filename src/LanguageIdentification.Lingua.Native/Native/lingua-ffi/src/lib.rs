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

#[no_mangle]
pub unsafe extern "C" fn lingua_detect_single(
    detector: &LanguageDetector,
    text_ptr: *const c_char,
    text_len: size_t,
    result: *mut DetectionResult,
) -> LinguaStatus {
    let text = core::slice::from_raw_parts(text_ptr as *const u8, text_len);
    detect_single_internal(&detector, &text, result)
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
