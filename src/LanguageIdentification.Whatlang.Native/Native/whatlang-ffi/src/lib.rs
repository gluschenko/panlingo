extern crate libc;
extern crate whatlang;

use libc::size_t;
use std::os::raw::c_char;
use std::ptr;
use whatlang::{detect, detect_script, Lang, Script};

#[repr(C)]
pub enum WhatlangStatus {
    Ok = 0,
    DetectFailure = 1,
    BadTextPtr = 2,
    BadOutputPtr = 3,
    BadEnumValue = 4,
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

#[repr(C)]
#[derive(Debug)]
pub struct WhatlangPredictionResult {
    lang: u8,
    script: u8,
    confidence: f64,
    is_reliable: bool,
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_detect(ptr: *const c_char, len: size_t, result: *mut WhatlangPredictionResult) -> WhatlangStatus {
    if ptr.is_null() && len > 0 {
        return WhatlangStatus::BadTextPtr;
    }

    let text = if len == 0 { &[] } else { std::slice::from_raw_parts(ptr as *const u8, len) };
    detect_internal(text, result)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_detect_script(ptr: *const c_char, len: size_t, result: *mut u8) -> WhatlangStatus {
    if ptr.is_null() && len > 0 {
        return WhatlangStatus::BadTextPtr;
    }

    let text = if len == 0 { &[] } else { std::slice::from_raw_parts(ptr as *const u8, len) };
    detect_script_internal(text, result)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_lang_eng_name(lang: u8, result: *mut c_char, result_len: size_t) -> size_t {
    let Some(x) = lang_from_u8(lang) else { return usize::MAX; };
    copy_string(x.eng_name(), result, result_len)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_lang_code(lang: u8, result: *mut c_char, result_len: size_t) -> size_t {
    let Some(x) = lang_from_u8(lang) else { return usize::MAX; };
    copy_string(x.code(), result, result_len)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_lang_name(lang: u8, result: *mut c_char, result_len: size_t) -> size_t {
    let Some(x) = lang_from_u8(lang) else { return usize::MAX; };
    copy_string(x.name(), result, result_len)
}

#[no_mangle]
pub unsafe extern "C" fn whatlang_script_name(script: u8, result: *mut c_char, result_len: size_t) -> size_t {
    let Some(x) = script_from_u8(script) else { return usize::MAX; };
    copy_string(x.name(), result, result_len)
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
                        (*result).lang = lang_to_u8(info.lang());
                        (*result).script = script_to_u8(info.script());
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

fn detect_script_internal(text: &[u8], result: *mut u8) -> WhatlangStatus {
    if result == ptr::null_mut() {
        return WhatlangStatus::BadOutputPtr;
    }

    match std::str::from_utf8(text) {
        Ok(s) => {
            let res = detect_script(s);
            match res {
                Some(info) => {
                    unsafe {
                        *result = script_to_u8(info.to_owned());
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

unsafe fn copy_string(source: &str, destination: *mut c_char, destination_len: size_t) -> size_t {
    let len = source.len();
    if destination != ptr::null_mut() && destination_len > len {
        let src = source.as_ptr().cast::<c_char>();
        src.copy_to_nonoverlapping(destination, len);
        *destination.add(len) = 0;
    }
    len
}

fn lang_from_u8(value: u8) -> Option<Lang> {
    Some(match value {
        0 => Lang::Epo,
        1 => Lang::Eng,
        2 => Lang::Rus,
        3 => Lang::Cmn,
        4 => Lang::Spa,
        5 => Lang::Por,
        6 => Lang::Ita,
        7 => Lang::Ben,
        8 => Lang::Fra,
        9 => Lang::Deu,
        10 => Lang::Ukr,
        11 => Lang::Kat,
        12 => Lang::Ara,
        13 => Lang::Hin,
        14 => Lang::Jpn,
        15 => Lang::Heb,
        16 => Lang::Yid,
        17 => Lang::Pol,
        18 => Lang::Amh,
        19 => Lang::Jav,
        20 => Lang::Kor,
        21 => Lang::Nob,
        22 => Lang::Dan,
        23 => Lang::Swe,
        24 => Lang::Fin,
        25 => Lang::Tur,
        26 => Lang::Nld,
        27 => Lang::Hun,
        28 => Lang::Ces,
        29 => Lang::Ell,
        30 => Lang::Bul,
        31 => Lang::Bel,
        32 => Lang::Mar,
        33 => Lang::Kan,
        34 => Lang::Ron,
        35 => Lang::Slv,
        36 => Lang::Hrv,
        37 => Lang::Srp,
        38 => Lang::Mkd,
        39 => Lang::Lit,
        40 => Lang::Lav,
        41 => Lang::Est,
        42 => Lang::Tam,
        43 => Lang::Vie,
        44 => Lang::Urd,
        45 => Lang::Tha,
        46 => Lang::Guj,
        47 => Lang::Uzb,
        48 => Lang::Pan,
        49 => Lang::Aze,
        50 => Lang::Ind,
        51 => Lang::Tel,
        52 => Lang::Pes,
        53 => Lang::Mal,
        54 => Lang::Ori,
        55 => Lang::Mya,
        56 => Lang::Nep,
        57 => Lang::Sin,
        58 => Lang::Khm,
        59 => Lang::Tuk,
        60 => Lang::Aka,
        61 => Lang::Zul,
        62 => Lang::Sna,
        63 => Lang::Afr,
        64 => Lang::Lat,
        65 => Lang::Slk,
        66 => Lang::Cat,
        67 => Lang::Tgl,
        68 => Lang::Hye,
        _ => return None,
    })
}

fn lang_to_u8(value: Lang) -> u8 {
    match value {
        Lang::Epo => 0,
        Lang::Eng => 1,
        Lang::Rus => 2,
        Lang::Cmn => 3,
        Lang::Spa => 4,
        Lang::Por => 5,
        Lang::Ita => 6,
        Lang::Ben => 7,
        Lang::Fra => 8,
        Lang::Deu => 9,
        Lang::Ukr => 10,
        Lang::Kat => 11,
        Lang::Ara => 12,
        Lang::Hin => 13,
        Lang::Jpn => 14,
        Lang::Heb => 15,
        Lang::Yid => 16,
        Lang::Pol => 17,
        Lang::Amh => 18,
        Lang::Jav => 19,
        Lang::Kor => 20,
        Lang::Nob => 21,
        Lang::Dan => 22,
        Lang::Swe => 23,
        Lang::Fin => 24,
        Lang::Tur => 25,
        Lang::Nld => 26,
        Lang::Hun => 27,
        Lang::Ces => 28,
        Lang::Ell => 29,
        Lang::Bul => 30,
        Lang::Bel => 31,
        Lang::Mar => 32,
        Lang::Kan => 33,
        Lang::Ron => 34,
        Lang::Slv => 35,
        Lang::Hrv => 36,
        Lang::Srp => 37,
        Lang::Mkd => 38,
        Lang::Lit => 39,
        Lang::Lav => 40,
        Lang::Est => 41,
        Lang::Tam => 42,
        Lang::Vie => 43,
        Lang::Urd => 44,
        Lang::Tha => 45,
        Lang::Guj => 46,
        Lang::Uzb => 47,
        Lang::Pan => 48,
        Lang::Aze => 49,
        Lang::Ind => 50,
        Lang::Tel => 51,
        Lang::Pes => 52,
        Lang::Mal => 53,
        Lang::Ori => 54,
        Lang::Mya => 55,
        Lang::Nep => 56,
        Lang::Sin => 57,
        Lang::Khm => 58,
        Lang::Tuk => 59,
        Lang::Aka => 60,
        Lang::Zul => 61,
        Lang::Sna => 62,
        Lang::Afr => 63,
        Lang::Lat => 64,
        Lang::Slk => 65,
        Lang::Cat => 66,
        Lang::Tgl => 67,
        Lang::Hye => 68,
    }
}

fn script_from_u8(value: u8) -> Option<Script> {
    Some(match value {
        0 => Script::Arabic,
        1 => Script::Armenian,
        2 => Script::Bengali,
        3 => Script::Cyrillic,
        4 => Script::Devanagari,
        5 => Script::Ethiopic,
        6 => Script::Georgian,
        7 => Script::Greek,
        8 => Script::Gujarati,
        9 => Script::Gurmukhi,
        10 => Script::Hangul,
        11 => Script::Hebrew,
        12 => Script::Hiragana,
        13 => Script::Kannada,
        14 => Script::Katakana,
        15 => Script::Khmer,
        16 => Script::Latin,
        17 => Script::Malayalam,
        18 => Script::Mandarin,
        19 => Script::Myanmar,
        20 => Script::Oriya,
        21 => Script::Sinhala,
        22 => Script::Tamil,
        23 => Script::Telugu,
        24 => Script::Thai,
        _ => return None,
    })
}

fn script_to_u8(value: Script) -> u8 {
    match value {
        Script::Arabic => 0,
        Script::Armenian => 1,
        Script::Bengali => 2,
        Script::Cyrillic => 3,
        Script::Devanagari => 4,
        Script::Ethiopic => 5,
        Script::Georgian => 6,
        Script::Greek => 7,
        Script::Gujarati => 8,
        Script::Gurmukhi => 9,
        Script::Hangul => 10,
        Script::Hebrew => 11,
        Script::Hiragana => 12,
        Script::Kannada => 13,
        Script::Katakana => 14,
        Script::Khmer => 15,
        Script::Latin => 16,
        Script::Malayalam => 17,
        Script::Mandarin => 18,
        Script::Myanmar => 19,
        Script::Oriya => 20,
        Script::Sinhala => 21,
        Script::Tamil => 22,
        Script::Telugu => 23,
        Script::Thai => 24,
    }
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

