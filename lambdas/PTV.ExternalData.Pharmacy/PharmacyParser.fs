namespace PTV.ExternalData.Pharmacy

open System
open System.Text.RegularExpressions
open PTV.ExternalData.Pharmacy.Utils

/// This module contains the basic functions for parsing a Pharmacy XML into a ServiceLocation JSON.
/// Specialized parsers for opening hours and languages can be found in separate modules.
module PharmacyParser =
  
  /// <summary>Removes whitespaces from the whole string.</summary>
  /// <param name="text">Input string to remove whitespaces.</param>
  /// <returns>Input string without whitespaces.</returns>
  let removeWhitespaces text =
    Regex.Replace(text, @"\s+", "")
    
  /// <summary>Wraps a string url into the WebPage object expected by Open API. Prefixes the URL with http, if needed.</summary>
  /// <param name="url">Raw url string.</param>
  /// <returns>A WebPages json object with formatted URL and Finnish as language.</returns>
  let parseWebPage (url: string) =
    let formattedUrl =  if url.ToLower().StartsWith("http")
                        then url
                        else "http://" + url
    ServiceLocationJson.WebPage
      (
        url = formattedUrl,
        value = "",
        language = Defaults.language
      )

  let parseWebPages (row: string option) =
    match row with
    | Some value ->
      // Split the input by semicolon
      let result = value.Split(';')
                    // Filter out all empty substrings
                    |> Seq.filter (fun substr -> not(System.String.IsNullOrWhiteSpace(substr)))
                    |> Seq.map (fun value -> value |> removeWhitespaces |> parseWebPage)
                    |> Seq.toArray
      result
    | None -> [||]

  /// <summary>Formats a fax number and wraps it into a FaxNumber object.</summary>
  /// <param name="faxNumber">The unformatted fax number string.</param>
  /// <returns>A FaxNumber object with Finnish prefix and language.</returns>
  let parseFaxNumber (faxNumber: string) =
    // Selects only digits from the input string.
    let pureNumber = faxNumber.ToCharArray()
                      |> Array.filter (fun item -> item |> Char.IsDigit)
                      |> String
    ServiceLocationJson.FaxNumber
      (
        prefixNumber = Defaults.prefix,
        isFinnishServiceNumber = false,
        language = Defaults.language,
        number = pureNumber
      )

  /// <summary>Parses a fax number into a list of fax numbers.</summary>
  /// <param name="faxNumber">An optional fax number string.</param>
  /// <returns>An empty array, if the fax number is None. Or an array of one fax number, if it is Some.</returns>
  let parseFaxNumbers (faxNumber: string option) =
    // The match operator in F# is really similar to switch in C#.
    // In fact, a lot of the pattern matching functionality for switch that was introduced in new versions of C#
    // have been originally developed for the match expression in F#.
    match faxNumber with
    | Some value -> [| parseFaxNumber value |]
    | None -> [||]
    
    
  /// <summary>Check that text length of input is greater then minInputLength, otherwise add default text</summary>
  /// <param name="input">A input string.</param>
  /// <param name="minInputLength">A number of min length of input string.</param>
  /// <param name="defaultValue">A string default text.</param>
  /// <returns>String which length is greater then requested minimum length.</returns>
  let checkMinLengthAddMissingText (input: string) (minInputLength: int) (defaultValue: string) =
    if input.Length > 0 && input.Length < minInputLength 
      then input + " " + defaultValue
      else input
      
  /// <summary>Check that text length of input isn't greater then maxLength, otherwise value is cropped.</summary>
  /// <param name="input">A input string.</param>
  /// <param name="maxInputLength">A number of min length of input string.</param>
  /// <returns>String which length isn't greater then set of maximum length.</returns>
  let checkMaxLengthCropExceededValue (input: string) (maxInputLength: int) =
    if input.Length > 0 && maxInputLength > 0 && input.Length > maxInputLength 
      then input.Substring(0, maxInputLength).Trim()
      else input
    
  /// <summary>Parses a phone number and related charge information.</summary>
  /// <param name="phoneNumber"></param>
  /// <param name="chargeInfo"></param>
  /// <returns></returns>
  let parsePhoneNumber (phoneNumber: string option) (chargeInfo: string option) =
    match phoneNumber with
    | None -> [||]
    | Some value ->
      match value with
      | Regex @"\(?\s*\d+\s*\)?\s*[\d*\s*]*" pureNumber ->
        let alternativeChargeInfo = value.[pureNumber.Length..]
        let chargeInfoValue = chargeInfo |? alternativeChargeInfo
        let minLengthOfChargeInfoValue = 5
        
        let chargeType = match chargeInfoValue with
                          | EmptyString _ -> Defaults.phoneCharged
                          | _ -> Defaults.phoneFree
        [|
          ServiceLocationJson.PhoneNumber
            (
              additionalInformation = "",
              serviceChargeType = chargeType,
              chargeDescription = checkMinLengthAddMissingText chargeInfoValue minLengthOfChargeInfoValue chargeType,
              prefixNumber = Defaults.prefix,
              isFinnishServiceNumber = false,
              number = removeWhitespaces pureNumber,
              language = Defaults.language
            )
        |]
      | _ -> [||]

  /// <summary>Removes dashes and additional information from a street number.</summary>
  /// <param name="input">Raw street number.</param>
  /// <returns>A clean street number without additional information.</returns>
  let parseStreetNumber (input: string) =
    if input.Contains("-") && not (Regex.Match(input, @"\d+\s*\-?\s*\d+").Success)
    then input.Substring(0, input.IndexOf("-") - 1).Trim()
    else input

  /// <summary>Combines street name, number and postal code into a StreetAddress json object for the Open API.</summary>
  /// <param name="streetName">A string street name.</param>
  /// <param name="streetNumber">A string street number.</param>
  /// <param name="postalCode">A string postal code.</param>
  /// <returns>A StreetAddress json object for the Open API with Finnish as the default language.</returns>
  let parseStreetAddress streetName streetNumber postalCode =
    ServiceLocationJson.StreetAddress
      (
        street = [| ServiceLocationJson.LocalizedText(value = streetName, language = Defaults.language) |],
        streetNumber = streetNumber,
        postalCode = removeWhitespaces postalCode,
        municipality = "",
        latitude = "",
        longitude = "",
        additionalInformation = [||]
      )

  /// <summary>Parses the Kayntiosoite XML element into an Address json object for the Open API.</summary>
  /// <param name="input">A Kayntiosoite XML element.</param>
  /// <returns>An Address of type Location/Single and given street, number and postal code.</returns>
  let parseAddress (input: PharmaciesXml.Kayntiosoite) =
    let streetAndNumber = input.Katuosoite
    let index = streetAndNumber.IndexOfAny("0123456789 ,".ToCharArray())

    let (streetName, numberText) =
      if index < 0
      then (streetAndNumber.Trim(), "")
      else (streetAndNumber.Substring(0, index).Trim(), streetAndNumber.Substring(index + 1).Trim())

    let streetNumber = parseStreetNumber numberText

    ServiceLocationJson.Address
      (
        ``type`` = Defaults.addressLocation,
        subType = Defaults.addressSingle,
        postOfficeBoxAddress = None,
        otherAddress = None,
        locationAbroad = [||],
        country = None,
        streetAddress = Some(parseStreetAddress streetName streetNumber (input.Postinumero.String |? ""))
      )

  /// <summary>Creates a wrapper structure around a string literal that should be a name or a description.</summary>
  /// <param name="input">The input string.</param>
  /// <param name="stringType">The type of the string. For names it is Name, AleternativeName.
  /// For descriptions Summary or Description.</param>
  /// <returns>A NameOrDescription object with Finnish as the language.</returns>
  let parseNameOrDescription input stringType maxlength =
    
    ServiceLocationJson.NameOrDescription
      (
        value =  checkMaxLengthCropExceededValue input maxlength,
        ``type`` = stringType,
        language = Defaults.language
      )

  /// <summary>Takes a single XML pharmacy as an input and converts it to a ServiceLocation JSON object.</summary>
  /// <param name="secrets">AWS secret values.</param>
  /// <param name="input">A pharmacy XML object.</param>
  /// <returns>Converted ServiceLocation.</returns>
  let parsePharmacy (secrets: SecretsJson.Secret) (input: PharmaciesXml.Apteekki) =
    // Note, that F# does not use curly braces {} to limit the scope as C#. Instead, F# relies on indentation.
    // This can be a bit tricky from the start, as small changes in indentation can cause the program to
    // throw compilation errors, or behave strangely during runtime.
    
    // Also, in F# there is no notion of null. All fields must be instantiated explicitly. To enter an empty
    // value, F# uses the Option type, which can have two values - Some<T> or None (as null).
    ServiceLocationJson.ServiceLocation
      (
        // Unfortunately, the FSharp.Data library is not perfect. It sometimes gives strange names to some
        // of the generated fields. To change this behavior, you need to add some unused template fields.
        // This is the case of the "nameOrDescription" and "localizedText" fields, which do not exist in
        // the ServiceLocation Open API model, but they had to be added in order to override the FSharp.Data
        // naming. They are both set to None, which marks them as null in the resulting JSON.
        nameOrDescription = None,
        localizedText = None,
        sourceId = Some(buildSourceId input.Kelanumero),
        oid = None,
        organizationId = secrets.PharmaOrganizationId,
        serviceChannelNames = [| parseNameOrDescription input.Nimi "Name" 100 |],
        serviceChannelDescriptions =
          [|
            parseNameOrDescription (input.Apteekissaon |? (input.Nimi+" "+Defaults.description)) "Description" 2500
            parseNameOrDescription (input.Nimi+" "+ Defaults.summary) "Summary" 150
          |],
        displayNameType = [|
          // "type" is a keyword in F#. To use it as field name, you have to enclose it in double backticks (````).
          // The C# equivalent would be @type.
          ServiceLocationJson.DisplayNameType(``type`` = "Name", language = Defaults.language)
        |],
        areaType = Defaults.areaType,
        areas = [||],
        emails = [||],
        faxNumbers = parseFaxNumbers input.Faksi,
        phoneNumbers = parsePhoneNumber input.Puhelin input.PuhelunHinta,
        languages = LanguageParser.parseLanguages (input.Palvelukielet |? ""),
        webPages = parseWebPages input.Kotisivut,
        addresses = [| parseAddress input.Kayntiosoite |],
        serviceHours = OpeningHoursParser.parseServiceHours input.Aukioloajat,
        publishingStatus = Defaults.publishingStatus,
        isVisibleForAll = true,
        services = [| secrets.PharmaServiceId |],
        validFrom = None,
        validTo = None
      )

  /// <summary>Takes an input string, parses it to the XML object and then converts into a list of ServiceLocation
  /// json objects.</summary>
  /// <param name="secrets">AWS secret values.</param>
  /// <param name="input">String containing an XML of pharmacies.</param>
  /// <returns>List of ServiceLocations.</returns>
  let parsePharmacies (secrets: SecretsJson.Secret) input =
    // Use the FSharp.Data type to parse the input string into an object.
    let pharmacies = PharmaciesXml.Parse(input)
    // C# equivalent: pharmacies.Apteekkis.Select(x => parsePharmacy(secrets, x)).ToList()
    pharmacies.Apteekkis
    |> Seq.map (parsePharmacy secrets)
    |> Seq.toList
