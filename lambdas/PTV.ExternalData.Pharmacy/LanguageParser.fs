namespace PTV.ExternalData.Pharmacy

/// This module contains a mapping from Finnish language names into language codes.
module LanguageParser =
  
  /// <summary>Maps a Finnish language name into a language code.</summary>
  /// <param name="language">Finnish language name.</param>
  /// <returns>Language code, or "fi" as default.</returns>
  let mapLanguageToCode (language: string) =
    let lower = language.Trim().ToLowerInvariant()
    match lower with
    | "albania" ->  "sq"
    | "amhara" ->  "am"
    | "arabia" ->  "ar"
    | "armenia" ->  "hy"
    | "azeri" ->  "az"
    | "bengali" ->  "bn"
    | "bosnia" ->  "bs"
    | "bulgaria" ->  "bg"
    | "burma" ->  "my"
    | "englanti" ->  "en"
    | "espanja" ->  "es"
    | "farsi" ->  "fa"
    | "georgia" ->  "ka"
    | "heprea" ->  "he"
    | "hindi" ->  "hi"
    | "hollanti" ->  "nl"
    | "inarinsaame" ->  "smn"
    | "indonesia" ->  "id"
    | "islanti" ->  "is"
    | "italia" ->  "it"
    | "japani" ->  "ja"
    | "kazakki" ->  "kk"
    | "khmer" ->  "km"
    | "kiina" ->  "zh"
    | "kirgiisi" ->  "ky"
    | "koltansaame" ->  "sms"
    | "kongo" ->  "kg"
    | "korea" ->  "ko"
    | "kreikka" ->  "el"
    | "kroatia" ->  "hr"
    | "kurdi" ->  "ku"
    | "lao" ->  "lo"
    | "latvia" ->  "lv"
    | "liettua" ->  "lt"
    | "lingala" ->  "ln"
    | "makedonia" ->  "mk"
    | "malaiji" ->  "ms"
    | "nepali" ->  "ne"
    | "norja" ->  "no"
    | "paštu" ->  "ps"
    | "pohjoissaame" ->  "se"
    | "portugali" ->  "pt"
    | "puola" ->  "pl"
    | "ranska" ->  "fr"
    | "romania" ->  "ro"
    | "romanikieli" ->  "rom"
    | "ruanda" ->  "rw"
    | "rundi" ->  "rn"
    | "ruotsi" ->  "sv"
    | "saksa" ->  "de"
    | "serbia" ->  "sr"
    | "serbokroaatti" ->  "sh"
    | "slovakki" ->  "sk"
    | "sloveeni" ->  "sl"
    | "somali" ->  "so"
    | "suomi" ->  "fi"
    | "swahili" ->  "sw"
    | "tagalog" ->  "tl"
    | "tamili" ->  "ta"
    | "tanska" ->  "da"
    | "thai" ->  "th"
    | "tšekki" ->  "cs"
    | "turkki" ->  "tr"
    | "turkmeeni" ->  "tk"
    | "uiguuri" ->  "ug"
    | "ukraina" ->  "uk"
    | "unkari" ->  "hu"
    | "urdu" ->  "ur"
    | "uzbekki" ->  "uz"
    | "venäjä" ->  "ru"
    | "vietnam" ->  "vi"
    | "viittomakieli" ->  "sgn-fi"
    | "viro" ->  "et"
    | "wolof" ->  "wo"
    | _ -> "fi"
    
  /// <summary>Parses a string containing multiple language names into an array of language codes.</summary>
  /// <param name="message">A string containing multiple language names.</param>
  /// <returns>An array of language codes.</returns>
  let parseLanguages (languages: string) =
    languages.Split(';')
    |> Seq.map mapLanguageToCode
    |> Seq.toArray
