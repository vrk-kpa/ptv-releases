namespace PTV.ExternalData.Pharmacy

open System
open System.Text.RegularExpressions
open PTV.ExternalData.Pharmacy.Utils

/// This module contains Regex patterns and functions for parsing opening hours.
module OpeningHoursParser =
  // Regex patterns for days and hours.
  let dayOfWeekShort = "\\bma|\\bti|\\bke|\\bto|\\bpe|\\bla|\\bsu"
  let dayOfWeekText = "maanantai(sin\\b|na\\b|\\b)|tiistai(sin\\b|na\\b|\\b)|keskiviikko(sin\\b|na\\b|\\b)|torstai(sin\\b|na\\b|\\b)|perjantai(sin\\b|na\\b|\\b)|lauantai(sin\\b|na\\b|\\b)|sunnuntai(sin\\b|na\\b|\\b)"
  let dayOfWeekAll = dayOfWeekShort + "|" + dayOfWeekText
  let dayOfWeekFrom = "(ma|ti|ke|to|pe|la|su)\\-"
  let dayOfWeekTo = "\\-(ma|ti|ke|to|pe|la|su)"
  let timeFrom = "\\d+\\.?\\d*\\-"
  let timeTo = "\\-\\d+\\.?\\d*"
  
  /// Complex regex patterns to match the whole day and hours string.
  let rule1 = @"(" + dayOfWeekAll + @")\-(" + dayOfWeekAll + @")\s?\d+\.?\d*\-\d+\.?\d*"
  let rule2 = @"(" + dayOfWeekAll + @")\s?\d+\.?\d*\-\d+\.?\d*"
  let rule3 = @"(" + dayOfWeekAll + @")\b"  
  
  /// <summary>Parses a text containing Finnish day names into a value
  /// from the Weekday discriminated union.</summary>
  /// <param name="text">The input string option.</param>
  /// <returns>None, if the input string is empty or could not be parsed.
  /// Otherwise, returns the corresponding Weekday value.</returns>  
  let parseDayOfWeek (text: string option) =
    match text with
    | Some value ->
      match value with
      | "ma" | "maanantai" | "maanantaisin" | "maanantaina" -> Some Monday
      | "ti" | "tiistai" | "tiistaisin" | "tiistaina" -> Some Tuesday
      | "ke" | "keskiviikko" | "keskiviikkosin" | "keskiviikkona" -> Some Wednesday
      | "to" | "torstai" | "torstaisin" | "torstaina" -> Some Thursday
      | "pe" | "perjantai" | "perjantaisin" | "perjantaina" -> Some Friday
      | "la" | "lauantai" | "lauantaisin" | "lauantaina" -> Some Saturday
      | "su" | "sunnuntai" | "sunnuntaisin" | "sunnuntaina" -> Some Sunday
      | _ -> None
    | _ -> None
    
  /// <summary>Tries to apply given pattern on tested string. If it matches, the function removes the unwanted
  /// characters specified in the removePattern and returns the resulting text.</summary>
  /// <param name="text">Tested string.</param>
  /// <param name="pattern">Pattern to match.</param>
  /// <param name="removePattern">Unwanted characters that should be removed from the text.</param>
  /// <returns>None, if the text does not match the pattern. Otherwise, returns the matching text
  /// with removed unwanted characters.</returns>
  let executePattern text pattern removePattern =
    match text with
    | Regex pattern result -> Some(Regex.Replace(result, removePattern, String.Empty).Trim())
    | _ -> None
    
  /// <summary>Processes a text that matches the rule1 regex pattern.</summary>
  /// <param name="value">The text that should be parsed.</param>
  /// <returns>An OpeningTimeOption record with parsed values.</returns>
  let processRule1 value =
    let dayFrom = executePattern value dayOfWeekFrom @"\-" |> parseDayOfWeek
    let dayTo = executePattern value dayOfWeekTo @"\-" |> parseDayOfWeek
    let timeFrom = executePattern value timeFrom @"\-"
    let timeTo = executePattern value timeTo @"\-"
    // This is the most confusing syntax in F#. By default, F# returns the last statement in the function body.
    // If nothing should be returned or the statement is ignored (|> ignore), the it returns a unit (like void).
    // However, in async functions, you have to use the return (or return!) keyword, to explicitly say, what is
    // being returned.
    //
    // This function return a record. This is denoted by the curly braces. F# usually automatically detects, what
    // type of record is being returned. However, in this case OpeningTimeOption and OpeningTimeValue records are
    // very similar, so F# is confused. In normal languages, you would specify the type of record like this:
    //   OpeningTimeOption { DayFrom = ... }
    // or like this:
    //   { DayFrom = ... } : OpeningTimeOption
    // However, in F#, the record type is specified by prefixing the first record member, thus we end up with this:
    { OpeningTimeOption.DayFrom = dayFrom; DayTo = dayTo; TimeFrom = timeFrom; TimeTo = timeTo; }
    
  /// <summary>Processes a text that matches the rule2 regex pattern.</summary>
  /// <param name="value">The text that should be parsed.</param>
  /// <returns>An OpeningTimeOption record with parsed values.</returns>
  let processRule2 value =
    let dayFrom = executePattern value dayOfWeekAll "" |> parseDayOfWeek
    let timeFrom = executePattern value timeFrom @"\-"
    let timeTo = executePattern value timeTo @"\-"
    // Returns an OpeningTimeOption record, where the DayFrom and DayTo are the same.
    { OpeningTimeOption.DayFrom = dayFrom; DayTo = dayFrom; TimeFrom = timeFrom; TimeTo = timeTo; }
    
  /// <summary>Processes a text that matches the rule3 regex pattern.</summary>
  /// <param name="value">The text that should be parsed.</param>
  /// <returns>An OpeningTimeOption record with parsed values.</returns>
  let processRule3 value =
    let dayFrom = Some value |> parseDayOfWeek
    // Returns an OpeningTimeOption record, where the DayFrom and DayTo are the same and no hours are specified.
    { OpeningTimeOption.DayFrom = dayFrom; DayTo = dayFrom; TimeFrom = None; TimeTo = None; }    
  
  /// <summary>Tries to match given opening time substring with one of the three regex pattern rules.
  /// If it matches a certain rule, the string is processed. Otherwise, an empty record is returned.</summary>
  /// <param name="substr">Part of the opening time string that should be parsed.</param>
  /// <returns>An OpeningTimeOption record, which may have its values filled in or empty, based
  /// on the success of pattern matching and parsing.</returns>
  let regexOpeningHours (substr: string) =
    let rowItem = substr.Trim().ToLower()
    match rowItem with
    | Regex rule1 value -> processRule1 value
    | Regex rule2 value -> processRule2 value
    | Regex rule3 value -> processRule3 value
    | _ -> {  DayFrom = None; DayTo = None; TimeFrom = None; TimeTo = None; }
    
  /// <summary>Formats an hours or minutes substring to always have two digits.
  /// If the string has 1 digit, 0 is prefixed. If it has more than 2 digits, the first two are taken.
  /// If it has exactly 2 digits, the string is untouched.</summary>
  /// <param name="hours">The hours or minutes substring.</param>
  /// <returns>A string with exactly 2 characters.</returns>
  let extendToTwoDigits (hours: string) =
    match hours.Length with
    | 1 -> "0" + hours
    | 2 -> hours
    | _ -> hours.Substring(0, 2)
    
  /// <summary>Parses minutes to integer and then returns the closest smaller quarter as a string.</summary>
  /// <param name="text">Minutes to be parsed and rounded.</param>
  /// <returns>A string containing the values 00, 15, 30 or 45.</returns>
  let roundMinutes (text: string) =
    let minutes = text |> int
    match minutes with
    | value when value < 15 -> "00"
    | value when value < 30 -> "15"
    | value when value < 45 -> "30"
    | _ -> "45"   
    
  /// <summary>Formats a time string to match the HH:mm pattern.</summary>
  /// <param name="text">A string containing time information.</param>
  /// <returns>A string in the HH:mm format.</returns>
  let formatHourMinutes (text: string) =
    // If . is present, replace it with : and then split the string into hours and minutes.
    let parts = text.Replace('.', ':').Split(':', StringSplitOptions.RemoveEmptyEntries)
    // Format the hours part to always have 2 digits.
    let hours = extendToTwoDigits parts.[0]
    match parts.Length with
    // If only hours were specified, add default 00 minutes and return the value.
    | 1 -> hours + ":00"
    // Otherwise, format the minutes part to 2 digits, round it to the nearest quarter...
    | _ ->
      let minutes = parts.[1] |> extendToTwoDigits |> roundMinutes
      // ...and return the resulting string.
      hours + ":" + minutes
    
  /// <summary>Converts an OpeningTimeValue record into the OpeningHour json expected by Open API.</summary>
  /// <param name="recordHours">Parsed OpeningTimeValue record.</param>
  /// <returns>An OpeningHour json object.</returns>
  let mapToOpeningHours (recordHours: OpeningTimeValue) =
    ServiceLocationJson.OpeningHour
      (
        dayFrom = unionToString recordHours.DayFrom,
        dayTo = unionToString recordHours.DayTo,
        from = formatHourMinutes recordHours.TimeFrom,
        ``to`` = formatHourMinutes recordHours.TimeTo
      )
          
  /// <summary>Converts an OpeningTimeOption record into the OpeningTimeValue record. Any empty fields are replaced
  /// by the default values: Monday - Sunday, 00:00 - 23:45.</summary>
  /// <param name="source">Input OpeningTimeOption record.</param>
  /// <returns>A converted OpeningTimeValue record.</returns> 
  let convertHoursToValues (source: OpeningTimeOption) =
    {
      DayFrom = source.DayFrom |? Monday
      DayTo = source.DayTo |? Sunday
      TimeFrom = source.TimeFrom |? "00:00"
      TimeTo = source.TimeTo |? "23:00"
    }
  
  /// <summary>This function checks the opening time records and if only days are specified,
  /// it tries to find the nearest related opening time. E.g.:  ke, to, pe 10-17 is parsed into 
  /// three opening times: Wednesday and Thursday with no hours and Friday with 10:00 - 17:00.
  /// For Wednesday, the functions searches the next day which has time filled in (i.e.: Friday)
  /// and adds the same time to Wednesday.</summary>
  /// <param name="related">All opening times for the same pharmacy.</param>
  /// <param name="index">The index of the current opening time in the list of all opening times.</param>
  /// <param name="related">The current opening time.</param>
  /// <returns>The current opening time with filled-in hours, if any are found.</returns>    
  let polyfillHours (related: OpeningTimeOption list) (index: int) (current: OpeningTimeOption) =
    match current.DayFrom with
    | Some _ -> 
      match current.TimeFrom with
      // If both day and hours are filled in, do nothing
      // (just change the type from OpeningTimeOption to OpeningTimeValue).
      | Some _ -> convertHoursToValues current
      | None ->
        // If day is filled in, but hours not, try to find the next day with filled in hours
        let source = related.[index..] |> List.tryFind (fun item -> item.TimeFrom.IsSome && item.TimeTo.IsSome)
        // The Option.bind function is similar to the conditional access operator (?.) in C#. Basically, if source
        // is Some, return TimeFrom and TimeTo. If it is None, return None.
        let timeFrom = source |> Option.bind (fun item -> item.TimeFrom)
        let timeTo = source |> Option.bind (fun item -> item.TimeTo)
        // Return a record for the current day with the closest related hours, or default values.
        {
          DayFrom = current.DayFrom |? Monday
          DayTo = current.DayTo |? Sunday
          TimeFrom = timeFrom |? "00:00"
          TimeTo = timeTo |? "23:45"
        }
    // If neither days nor hours are specified, do nothing (just convert to OpeningTimeValue with default values).
    | None -> convertHoursToValues current
    
  /// <summary>Tries to determine, whether the pharmacy is open 24/7.</summary>
  /// <param name="recordHours">Set of all opening times for given pharmacy.</param>
  /// <returns>True, if the pharmacy is always open. Otherwise, returns false.</returns>
  let getIsAlwaysOpen (recordHours: OpeningTimeOption list) =
    let mutable hasMonday = false
    let mutable hasSunday = false
    let mutable hasNoHours = true
    let mutable hasOnlyEdgeHours = true
    
    // Go through all available records and check the following:
    for record in recordHours do
      // 1. At least one record has start day set to Monday
      match record.DayFrom with
      | Some Monday -> hasMonday <- true
      | _ -> ()
      
      // 2. At least one record has end day set to Sunday
      match record.DayTo with
      | Some Sunday -> hasSunday <- true
      | _ -> ()
      
      // 3. All records start at midnight, or have no opening hours.
      match record.TimeFrom with
      | Some "00:00" | Some "0" -> hasNoHours <- false
      | Some _ -> hasNoHours <- false; hasOnlyEdgeHours <- false
      | None -> hasOnlyEdgeHours <- false
      
      // 4. All records end at midnight, or have no opening hours.
      match record.TimeTo with
      | Some "24:00" | Some "24" | Some "00:00" | Some "0" -> hasNoHours <- false
      | Some _ -> hasNoHours <- false; hasOnlyEdgeHours <- false
      | None -> hasOnlyEdgeHours <- false
     
    // Return whether all 4 criteria have been met.
    hasMonday && hasSunday && (hasOnlyEdgeHours || hasNoHours)
   
  /// <summary>Converts a list of parsed OpeningTimeOptions into a ServiceHour json for the Open API.</summary>
  /// <param name="recordHours">The list of parsed OpeningTimeOptions.</param>
  /// <returns>A ServiceHour json for the Open API.</returns> 
  let mapToServiceHours (recordHours: OpeningTimeOption list) =
    // Determine, if the pharmacy is open 24/7.
    let isAlwaysOpen = getIsAlwaysOpen recordHours    
    ServiceLocationJson.ServiceHour
      (
        serviceHourType = Defaults.openingHoursType,
        validFrom = None,
        validTo = None,
        isClosed = Some false,
        validForNow = Some false,
        isAlwaysOpen = Some isAlwaysOpen,
        isReservation = Some false,
        additionalInformation = [||],
        openingHour = if isAlwaysOpen
                      then [||]
                      else recordHours
                           // If the pharmacy is not always open, remove empty entries from the parsed list
                           |> List.filter (fun item -> item.DayFrom.IsSome)
                           // Fill in missing hours to all days
                           |> List.mapi (polyfillHours recordHours)
                           // And map to the Open API json format
                           |> List.map mapToOpeningHours
                           |> List.toArray
      )
  
  /// <summary>Parses a string containing opening time information into an array of ServiceHour jsons.</summary>
  /// <param name="row">Optional string containing time information.</param>
  /// <returns></returns>
  let parseServiceHours (row: string option) =
    match row with
    | Some value ->
      // Split the input by comma
      let result = value.Split(',')
                    // Filter out all empty substrings
                    |> Seq.filter (fun substr -> not(System.String.IsNullOrWhiteSpace(substr)))
                    // Try matching a regex pattern on each of those substrings
                    |> Seq.map regexOpeningHours
                    |> Seq.toList
                    // Map the result to ServiceHour json
                    |> mapToServiceHours
      [| result |]
    | None -> [||]
