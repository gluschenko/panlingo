extern crate libc;
extern crate whatlang;

use libc::size_t;
use std::ffi::CStr;
use std::os::raw::c_char;
use std::ptr;
use whatlang::{detect, detect_lang, detect_script, Lang, Script};

#[repr(u8)]
pub enum WhatlangStatus {
    Ok = 0,
    DetectFailure = 1,
    BadTextPtr = 2,
    BadOutputPtr = 3,
}

#[repr(u8)]
#[derive(Debug, Copy, Clone)]
pub enum WhatlangLanguage {
    Epo = 0,
    Eng = 1,
    Rus = 2,
    Cmn = 3,
    Spa = 4,
    Por = 5,
    Ita = 6,
    Ben = 7,
    Fra = 8,
    Deu = 9,
    Ukr = 10,
    Kat = 11,
    Ara = 12,
    Hin = 13,
    Jpn = 14,
    Heb = 15,
    Yid = 16,
    Pol = 17,
    Amh = 18,
    Jav = 19,
    Kor = 20,
    Nob = 21,
    Dan = 22,
    Swe = 23,
    Fin = 24,
    Tur = 25,
    Nld = 26,
    Hun = 27,
    Ces = 28,
    Ell = 29,
    Bul = 30,
    Bel = 31,
    Mar = 32,
    Kan = 33,
    Ron = 34,
    Slv = 35,
    Hrv = 36,
    Srp = 37,
    Mkd = 38,
    Lit = 39,
    Lav = 40,
    Est = 41,
    Tam = 42,
    Vie = 43,
    Urd = 44,
    Tha = 45,
    Guj = 46,
    Uzb = 47,
    Pan = 48,
    Aze = 49,
    Ind = 50,
    Tel = 51,
    Pes = 52,
    Mal = 53,
    Ori = 54,
    Mya = 55,
    Nep = 56,
    Sin = 57,
    Khm = 58,
    Tuk = 59,
    Aka = 60,
    Zul = 61,
    Sna = 62,
    Afr = 63,
    Lat = 64,
    Slk = 65,
    Cat = 66,
    Tgl = 67,
    Hye = 68,
}

#[repr(u8)]
#[derive(Debug, Copy, Clone)]
pub enum WhatlangScript {
    Arab = 0,
    Armn = 1,
    Beng = 2,
    Cyrl = 3,
    Deva = 4,
    Ethi = 5,
    Geor = 6,
    Grek = 7,
    Gujr = 8,
    Guru = 9,
    Hang = 10,
    Hebr = 11,
    Hira = 12,
    Knda = 13,
    Kana = 14,
    Khmr = 15,
    Latn = 16,
    Mlym = 17,
    Mand = 18,
    Mymr = 19,
    Orya = 20,
    Sinh = 21,
    Taml = 22,
    Telu = 23,
    Thai = 24,
}

impl From<Lang> for WhatlangLanguage {
    fn from(language: Lang) -> Self {
        unsafe { std::mem::transmute(language as u8) }
    }
}

impl From<WhatlangLanguage> for Lang {
    fn from(ffi_language: WhatlangLanguage) -> Self {
        unsafe { std::mem::transmute(ffi_language as u8) }
    }
}

impl From<Script> for WhatlangScript {
    fn from(script: Script) -> Self {
        unsafe { std::mem::transmute(script as u8) }
    }
}

impl From<WhatlangScript> for Script {
    fn from(ffi_script: WhatlangScript) -> Self {
        unsafe { std::mem::transmute(ffi_script as u8) }
    }
}

#[repr(C)]
#[derive(Debug)]
pub struct WhatlangPredictionResult {
    lang: Lang,
    script: Script,
    confidence: f64,
    is_reliable: bool,
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_detect(ptr: *const c_char, result: *mut WhatlangPredictionResult) -> WhatlangStatus {
    let x = CStr::from_ptr(ptr);
    detect_internal(x.to_bytes(), result)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_detect_script(ptr: *const c_char, result: *mut WhatlangScript) -> WhatlangStatus {
    let x = CStr::from_ptr(ptr);
    detect_script_internal(x.to_bytes(), result)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_lang_eng_name(lang: WhatlangLanguage, result: *mut c_char) -> size_t {
    let x: Lang = lang.into();
    copy_string(x.eng_name(), result)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_lang_code(lang: WhatlangLanguage, result: *mut c_char) -> size_t {
    let x: Lang = lang.into();
    copy_string(x.code(), result)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_lang_name(lang: WhatlangLanguage, result: *mut c_char) -> size_t {
    let x: Lang = lang.into();
    copy_string(x.name(), result)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_script_name(script: WhatlangScript, result: *mut c_char) -> size_t {
    let x: Script = script.into();
    copy_string(x.name(), result)
}

fn detect_internal(text: &[u8], result: *mut WhatlangPredictionResult) -> WhatlangStatus {
    if result == ptr::null_mut() {
        return WhatlangStatus::BadOutputPtr;
    }

    match std::str::from_utf8(text) {
        Ok(s) => {
            let res = detect(s);
            match res {
                Some(info) => {
                    unsafe {
                        (*result).lang = info.lang();
                        (*result).script = info.script();
                        (*result).confidence = info.confidence();
                        (*result).is_reliable = info.is_reliable();
                    }
                    WhatlangStatus::Ok
                }
                None => {
                    // Could not detect language
                    WhatlangStatus::DetectFailure
                }
            }
        }
        Err(_) => {
            // Bad string pointer
            WhatlangStatus::BadTextPtr
        }
    }
}

fn detect_script_internal(text: &[u8], result: *mut WhatlangScript) -> WhatlangStatus {
    if result == ptr::null_mut() {
        return WhatlangStatus::BadOutputPtr;
    }

    match std::str::from_utf8(text) {
        Ok(s) => {
            let res = detect_script(s);
            match res {
                Some(info) => {
                    let x: WhatlangScript = info.to_owned().into();
                    unsafe {
                        *result = x;
                    }
                    WhatlangStatus::Ok
                }
                None => {
                    // Could not detect language
                    WhatlangStatus::DetectFailure
                }
            }
        }
        Err(_) => {
            // Bad string pointer
            WhatlangStatus::BadTextPtr
        }
    }
}

unsafe fn copy_string(source: &str, destination: *mut c_char) -> size_t {
    let len = source.len();
    if destination != ptr::null_mut() {
        let src = source.as_ptr().cast::<c_char>();
        src.copy_to_nonoverlapping(destination, len);
        *destination.add(len) = 0;
    }
    len
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test() {
        let text = "Привіт, як справи?";

        let prediction_result = detect(text);
        let lang_prediction_result = detect_lang(text);
        let script_prediction_result = detect_script(text);

        match prediction_result {
            None => {
                panic!("Failed!")
            }
            Some(x) => {
                println!("{}: {} ({})", x.lang().to_string(), x.confidence(), x.script().to_string());
                assert_eq!(x.lang(), Lang::Ukr);
                assert_eq!(x.script(), Script::Cyrillic);
            }
        }

        match lang_prediction_result {
            None => {
                panic!("Failed!")
            }
            Some(x) => {
                assert_eq!(x, Lang::Ukr);
            }
        }

        match script_prediction_result {
            None => {
                panic!("Failed!")
            }
            Some(x) => {
                assert_eq!(x, Script::Cyrillic);
            }
        }
    }
}

