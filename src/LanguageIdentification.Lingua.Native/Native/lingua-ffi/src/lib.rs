extern crate libc;
extern crate lingua;

use libc::size_t;
use std::os::raw::c_char;
use std::ptr;
use lingua::{Language, LanguageDetector, LanguageDetectorBuilder};

#[repr(u8)]
pub enum LinguaStatus {
    Ok = 0,
    DetectFailure = 1,
    BadTextPtr = 2,
    BadOutputPtr = 3,
    BadDetectorPtr = 4,
    BadEnumValue = 5,
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

#[repr(C)]
#[derive(Debug)]
pub struct LinguaPredictionResult {
    pub language: u8,
    pub confidence: f64,
}

#[repr(C)]
#[derive(Debug)]
pub struct LinguaPredictionRangeResult {
    pub language: u8,
    pub confidence: f64,
    pub start_index: u32,
    pub end_index: u32,
    pub word_count: u32,
}

#[repr(C)]
pub struct LinguaPredictionListResult {
    pub predictions: *const LinguaPredictionResult,
    pub predictions_count: u32,
}

#[repr(C)]
pub struct LinguaPredictionRangeListResult {
    pub predictions: *const LinguaPredictionRangeResult,
    pub predictions_count: u32,
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_code(
    language: u8,
    code: u8,
    buffer_ptr: *mut c_char,
    buffer_len: size_t,
) -> size_t { unsafe {
    let Some(x) = language_from_u8(language) else { return usize::MAX; };

    let Some(code) = language_code_from_u8(code) else { return usize::MAX; };

    let code = match code {
        LinguaLanguageCode::Alpha2 => {
            x.iso_code_639_1().to_string()
        },
        LinguaLanguageCode::Alpha3 => {
            x.iso_code_639_3().to_string()
        },
    };

    copy_cstr(&code, buffer_ptr, buffer_len)
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_language_detector_builder_create(
    languages: *const u8,
    language_count: size_t
) -> *mut LanguageDetectorBuilder { unsafe {
    if !languages.is_null() {
        let languages_slice = std::slice::from_raw_parts(languages, language_count);
        let mut languages = Vec::with_capacity(languages_slice.len());
        for language in languages_slice {
            let Some(language) = language_from_u8(*language) else {
                return ptr::null_mut();
            };
            languages.push(language);
        }
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
    detector: *const LanguageDetector,
    text: *const c_char,
    text_len: size_t,
    result: *mut LinguaPredictionListResult,
) -> LinguaStatus { unsafe {
    if detector.is_null() {
        return LinguaStatus::BadDetectorPtr;
    }
    if text.is_null() && text_len > 0 {
        return LinguaStatus::BadTextPtr;
    }

    let text = if text_len == 0 { &[] } else { std::slice::from_raw_parts(text as *const u8, text_len) };
    detect_single_internal(&*detector, text, result)
}}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn lingua_detect_mixed(
    detector: *const LanguageDetector,
    text: *const c_char,
    text_len: size_t,
    result: *mut LinguaPredictionRangeListResult,
) -> LinguaStatus { unsafe {
    if detector.is_null() {
        return LinguaStatus::BadDetectorPtr;
    }
    if text.is_null() && text_len > 0 {
        return LinguaStatus::BadTextPtr;
    }

    let text = if text_len == 0 { &[] } else { std::slice::from_raw_parts(text as *const u8, text_len) };
    detect_mixed_internal(&*detector, text, result)
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
                    language: language_to_u8(*language),
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
                    language: language_to_u8(x.language()),
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

unsafe fn copy_cstr(src: &str, dst: *mut c_char, dst_len: size_t) -> size_t { unsafe {
    let len = src.len();
    if dst != ptr::null_mut() && dst_len > len {
        let src = src.as_ptr().cast::<c_char>();
        src.copy_to_nonoverlapping(dst, len);
        *dst.add(len) = 0;
    }
    return len;
}}

fn language_code_from_u8(value: u8) -> Option<LinguaLanguageCode> {
    Some(match value {
        2 => LinguaLanguageCode::Alpha2,
        3 => LinguaLanguageCode::Alpha3,
        _ => return None,
    })
}

fn language_from_u8(value: u8) -> Option<Language> {
    Some(match value {
        0 => Language::Afrikaans,
        1 => Language::Albanian,
        2 => Language::Arabic,
        3 => Language::Armenian,
        4 => Language::Azerbaijani,
        5 => Language::Basque,
        6 => Language::Belarusian,
        7 => Language::Bengali,
        8 => Language::Bokmal,
        9 => Language::Bosnian,
        10 => Language::Bulgarian,
        11 => Language::Catalan,
        12 => Language::Chinese,
        13 => Language::Croatian,
        14 => Language::Czech,
        15 => Language::Danish,
        16 => Language::Dutch,
        17 => Language::English,
        18 => Language::Esperanto,
        19 => Language::Estonian,
        20 => Language::Finnish,
        21 => Language::French,
        22 => Language::Ganda,
        23 => Language::Georgian,
        24 => Language::German,
        25 => Language::Greek,
        26 => Language::Gujarati,
        27 => Language::Hebrew,
        28 => Language::Hindi,
        29 => Language::Hungarian,
        30 => Language::Icelandic,
        31 => Language::Indonesian,
        32 => Language::Irish,
        33 => Language::Italian,
        34 => Language::Japanese,
        35 => Language::Kazakh,
        36 => Language::Korean,
        37 => Language::Latin,
        38 => Language::Latvian,
        39 => Language::Lithuanian,
        40 => Language::Macedonian,
        41 => Language::Malay,
        42 => Language::Maori,
        43 => Language::Marathi,
        44 => Language::Mongolian,
        45 => Language::Nynorsk,
        46 => Language::Persian,
        47 => Language::Polish,
        48 => Language::Portuguese,
        49 => Language::Punjabi,
        50 => Language::Romanian,
        51 => Language::Russian,
        52 => Language::Serbian,
        53 => Language::Shona,
        54 => Language::Slovak,
        55 => Language::Slovene,
        56 => Language::Somali,
        57 => Language::Sotho,
        58 => Language::Spanish,
        59 => Language::Swahili,
        60 => Language::Swedish,
        61 => Language::Tagalog,
        62 => Language::Tamil,
        63 => Language::Telugu,
        64 => Language::Thai,
        65 => Language::Tsonga,
        66 => Language::Tswana,
        67 => Language::Turkish,
        68 => Language::Ukrainian,
        69 => Language::Urdu,
        70 => Language::Vietnamese,
        71 => Language::Welsh,
        72 => Language::Xhosa,
        73 => Language::Yoruba,
        74 => Language::Zulu,
        _ => return None,
    })
}

fn language_to_u8(value: Language) -> u8 {
    match value {
        Language::Afrikaans => 0,
        Language::Albanian => 1,
        Language::Arabic => 2,
        Language::Armenian => 3,
        Language::Azerbaijani => 4,
        Language::Basque => 5,
        Language::Belarusian => 6,
        Language::Bengali => 7,
        Language::Bokmal => 8,
        Language::Bosnian => 9,
        Language::Bulgarian => 10,
        Language::Catalan => 11,
        Language::Chinese => 12,
        Language::Croatian => 13,
        Language::Czech => 14,
        Language::Danish => 15,
        Language::Dutch => 16,
        Language::English => 17,
        Language::Esperanto => 18,
        Language::Estonian => 19,
        Language::Finnish => 20,
        Language::French => 21,
        Language::Ganda => 22,
        Language::Georgian => 23,
        Language::German => 24,
        Language::Greek => 25,
        Language::Gujarati => 26,
        Language::Hebrew => 27,
        Language::Hindi => 28,
        Language::Hungarian => 29,
        Language::Icelandic => 30,
        Language::Indonesian => 31,
        Language::Irish => 32,
        Language::Italian => 33,
        Language::Japanese => 34,
        Language::Kazakh => 35,
        Language::Korean => 36,
        Language::Latin => 37,
        Language::Latvian => 38,
        Language::Lithuanian => 39,
        Language::Macedonian => 40,
        Language::Malay => 41,
        Language::Maori => 42,
        Language::Marathi => 43,
        Language::Mongolian => 44,
        Language::Nynorsk => 45,
        Language::Persian => 46,
        Language::Polish => 47,
        Language::Portuguese => 48,
        Language::Punjabi => 49,
        Language::Romanian => 50,
        Language::Russian => 51,
        Language::Serbian => 52,
        Language::Shona => 53,
        Language::Slovak => 54,
        Language::Slovene => 55,
        Language::Somali => 56,
        Language::Sotho => 57,
        Language::Spanish => 58,
        Language::Swahili => 59,
        Language::Swedish => 60,
        Language::Tagalog => 61,
        Language::Tamil => 62,
        Language::Telugu => 63,
        Language::Thai => 64,
        Language::Tsonga => 65,
        Language::Tswana => 66,
        Language::Turkish => 67,
        Language::Ukrainian => 68,
        Language::Urdu => 69,
        Language::Vietnamese => 70,
        Language::Welsh => 71,
        Language::Xhosa => 72,
        Language::Yoruba => 73,
        Language::Zulu => 74,
    }
}

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
        let languages: Vec<u8> = Language::all()
            .into_iter()
            .map(language_to_u8)
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

            let status = lingua_detect_mixed(detector, text.as_ptr(), text.as_bytes().len(), result.as_mut_ptr());

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
