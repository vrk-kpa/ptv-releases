namespace PTV.ExternalData.Pharmacy

open System
open System.Collections.Generic
open System.Text.RegularExpressions
open PTV.ExternalData.Pharmacy.Utils

module CommonParser =      
  let buildLocationSourceId (id: int) =
    Defaults.locationIdPrefix + id.ToString()
    
  let buildEChannelSourceId (id: int) =
    Defaults.eChannelIdPrefix + id.ToString()
    
  let cropLongString (input: string) (maxInputLength: int) =
    if input.Length > 0 && maxInputLength > 0 && input.Length > maxInputLength 
    then input.Substring(0, maxInputLength).Trim()
    else input
    
  let fillShortString (input: string) (minInputLength: int) (defaultValue: string) =
    if input.Length > 0 && input.Length < minInputLength 
    then input + " " + defaultValue
    else input
    
  let createLocalizedText text maxLength =
    ServiceLocationJson.LocalizedText
      (
        value = cropLongString text maxLength,
        language = Defaults.language
      )
    
  let removeWhitespaces text =
    Regex.Replace(text, @"\s+", "")
    
  let removeLeadingZeroes (text: string) =
    text.TrimStart('0')
    
  let parseNameOrDescription input stringType maxlength =    
    ServiceLocationJson.NameOrDescription
      (
        value =  cropLongString input maxlength,
        ``type`` = stringType,
        language = Defaults.language
      )
    
  let parseDescription input stringType maxlength =    
    EChannelJson.ServiceChannelDescription
      (
        value =  cropLongString input maxlength,
        ``type`` = stringType,
        language = Defaults.language
      )
    
  let parseName input maxlength =    
    EChannelJson.ServiceChannelName
      (
        value =  cropLongString input maxlength,
        language = Defaults.language
      )
      
  let formatLocationWebPage (url: string) =
    let formattedUrl =  if url.ToLower().StartsWith("http")
                        then url
                        else "http://" + url
    ServiceLocationJson.WebPage
      (
        url = formattedUrl,
        value = "",
        language = Defaults.language
      )
      
  let formatEChannelWebPage (url: string) =
    let formattedUrl =  if url.ToLower().StartsWith("http")
                        then url
                        else "http://" + url
    let formattedWithDot = if formattedUrl.IndexOf(".") < 0
                           then formattedUrl + ".fi"
                           else formattedUrl
    parseName formattedWithDot Defaults.textLengths.WebPage

  let parseWebPages (row: string option) (formattingFunc: string -> 'a) =
    match row with
    | Some value ->
      // Split the input by semicolon
      let result = value.Split([| ';'; ',' |], StringSplitOptions.RemoveEmptyEntries)
                    // Filter out all empty substrings
                    |> Seq.map (fun value -> value |> removeWhitespaces |> formattingFunc)
                    |> Seq.toArray
      result
    | None -> [||]
    
  let parsePhoneNumber (phoneNumber: string) (chargeInfo: string option) =
    if String.IsNullOrWhiteSpace phoneNumber
    then [||]
    else
      match phoneNumber with
      | Regex @"\(?\s*\d+\s*\)?\s*[\d*\s*]*" pureNumber ->
        let alternativeChargeInfo = phoneNumber.[pureNumber.Length..]
        let chargeInfoValue = chargeInfo |? alternativeChargeInfo
        
        let chargeType = match chargeInfoValue with
                          | EmptyString _ -> Defaults.phoneCharged
                          | _ -> Defaults.phoneFree
        [|
          ServiceLocationJson.PhoneNumber
            (
              additionalInformation = "",
              serviceChargeType = chargeType,
              chargeDescription = fillShortString chargeInfoValue Defaults.textLengths.ChargeInfoMin chargeType,
              prefixNumber = Defaults.phonePrefix,
              isFinnishServiceNumber = false,
              number = (pureNumber |> removeWhitespaces |> removeLeadingZeroes),
              language = Defaults.language
            )
        |]
      | _ -> [||]
      
  let parseSupportPhone (phoneNumber: string) (chargeInfo: string option) =
    if String.IsNullOrWhiteSpace phoneNumber
    then [||]
    else
      match phoneNumber with
      | Regex @"\(?\s*\d+\s*\)?\s*[\d*\s*]*" pureNumber ->
        let alternativeChargeInfo = phoneNumber.[pureNumber.Length..]
        let chargeInfoValue = chargeInfo |? alternativeChargeInfo
        
        let chargeType = match chargeInfoValue with
                          | EmptyString _ -> Defaults.phoneCharged
                          | _ -> Defaults.phoneFree
        [|
          EChannelJson.SupportPhone
            (
              additionalInformation = "",
              serviceChargeType = chargeType,
              chargeDescription = fillShortString chargeInfoValue Defaults.textLengths.ChargeInfoMin chargeType,
              prefixNumber = Defaults.phonePrefix,
              isFinnishServiceNumber = false,
              number = (pureNumber |> removeWhitespaces |> removeLeadingZeroes),
              language = Defaults.language
            )
        |]
      | _ -> [||]
    
  let parseFaxNumber (faxNumber: string option) =
    let value = Option.defaultValue "" faxNumber
    let pureNumber = value.ToCharArray()
                      |> Array.filter Char.IsDigit
                      |> String    
    if String.IsNullOrWhiteSpace pureNumber
    then None
    else Some (ServiceLocationJson.FaxNumber
                (
                  prefixNumber = Defaults.phonePrefix,
                  isFinnishServiceNumber = false,
                  language = Defaults.language,
                  number = (pureNumber |> removeWhitespaces |> removeLeadingZeroes)
                ))
      
  let parseLanguages (languages: string option) (languageCodes: Dictionary<string, string>) =
    (Option.defaultValue Defaults.languageName languages).Split ";"
    |> Seq.map (fun lang ->
                            let language = lang.Trim().ToLowerInvariant()
                            if languageCodes.ContainsKey language
                            then Some languageCodes.[language]
                            else None)
    |> Seq.choose id
    |> Seq.toArray
        
  let parseAdditionalInfo info length =
    match info with
    | Some value ->
      if String.IsNullOrEmpty value
      then [||]
      else [| createLocalizedText value length |]
    | None -> [||]
    