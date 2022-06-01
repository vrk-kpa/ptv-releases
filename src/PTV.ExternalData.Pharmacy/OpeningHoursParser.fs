namespace PTV.ExternalData.Pharmacy

open System
open PTV.ExternalData.Pharmacy.Utils

module OpeningHoursParser =  
  let parseRegularDay from until day =
    match from with
    | None -> None
    | Some fromValue ->
      if String.IsNullOrWhiteSpace fromValue || fromValue.ToLowerInvariant() = Defaults.closed
      then None
      else 
        match until with
        | None -> None      
        | Some untilValue ->
          if String.IsNullOrWhiteSpace untilValue || untilValue.ToLowerInvariant() = Defaults.closed
          then None
          else Some (ServiceLocationJson.OpeningHour
                      (
                         dayFrom = day,
                         dayTo = day,
                         from = fromValue,
                         ``to`` = untilValue
                       ))
          
  let parseDaysOfTheWeek (input: PharmaciesXml.Aukioloaika) =
    (parseRegularDay input.MaanantaiAukeaa.String input.MaanantaiSulkeutuu.String (Weekday.Monday |> weekdayToString)) ::
    (parseRegularDay input.TiistaiAukeaa.String input.TiistaiSulkeutuu.String (Weekday.Tuesday |> weekdayToString)) ::
    (parseRegularDay input.KeskiviikkoAukeaa.String input.KeskiviikkoSulkeutuu.String (Weekday.Wednesday |> weekdayToString)) ::
    (parseRegularDay input.TorstaiAukeaa.String input.TorstaiSulkeutuu.String (Weekday.Thursday |> weekdayToString)) ::
    (parseRegularDay input.PerjantaiAukeaa.String input.PerjantaiSulkeutuu.String (Weekday.Friday |> weekdayToString)) ::
    (parseRegularDay input.LauantaiAukeaa.String input.LauantaiSulkeutuu.String (Weekday.Saturday |> weekdayToString)) ::
    (parseRegularDay input.SunnuntaiAukeaa.String input.SunnuntaiSulkeutuu.String (Weekday.Sunday |> weekdayToString)) :: []
    |> List.choose id
    
  let parseAdditionalInformationHour (input: string option) =
    ServiceLocationJson.ServiceHour
      (
        additionalInformation = CommonParser.parseAdditionalInfo input Defaults.textLengths.AdditionalInfo,
        isAlwaysOpen = false,
        isClosed = false,
        isReservation = false,
        validForNow = false,
        validFrom = None,
        validTo = None,
        serviceHourType = Defaults.serviceHoursType,
        openingHour = [||]
      )
  
  let parseNormalOpeningHour (input: PharmaciesXml.Aukioloaika) =
    seq {
      let normalHours = parseDaysOfTheWeek input    
      if List.isEmpty normalHours |> not
      then
        let header = Some input.Otsikko
        yield ServiceLocationJson.ServiceHour
                (
                  additionalInformation = CommonParser.parseAdditionalInfo header Defaults.textLengths.AdditionalInfo,
                  isAlwaysOpen = false,
                  isClosed = false,
                  isReservation = false,
                  validForNow = Option.isSome input.Loppupvm.Value,
                  validFrom = (Option.bind (fun value -> Some(DateTimeOffset(value))) input.Alkupvm.Value),
                  validTo = (Option.bind (fun value -> Some(DateTimeOffset(value))) input.Loppupvm.Value),
                  serviceHourType = Defaults.serviceHoursType,
                  openingHour = (List.toArray normalHours)
                )
        if input.Lisatietoja |> Option.defaultValue "" |> String.IsNullOrWhiteSpace |> not
        then yield parseAdditionalInformationHour input.Lisatietoja
    }
      
  let parseSpecialHours (infoTitle: string) (input: PharmaciesXml.Aukioloaika) (hoursType: string) =
    seq {
      let daysOfTheWeek = parseDaysOfTheWeek input
      let header =  if String.IsNullOrWhiteSpace input.Otsikko
                    then None
                    else Some (infoTitle +  ": " + input.Otsikko)
                    
      for openingHour in daysOfTheWeek do
        yield ServiceLocationJson.ServiceHour
                (
                  additionalInformation = CommonParser.parseAdditionalInfo header Defaults.textLengths.AdditionalInfo,
                  isAlwaysOpen = false,
                  isClosed = false,
                  isReservation = false,
                  validForNow = Option.isSome input.Loppupvm.Value,
                  validFrom = (Option.bind (fun value -> Some(DateTimeOffset(value))) input.Alkupvm.Value),
                  validTo = (Option.bind (fun value -> Some(DateTimeOffset(value))) input.Loppupvm.Value),
                  serviceHourType = hoursType,
                  openingHour = [| openingHour |]
                )
                  
      if input.Lisatietoja |> Option.defaultValue "" |> String.IsNullOrWhiteSpace |> not
      then yield parseAdditionalInformationHour input.Lisatietoja      
    }
    
  let parseHoliday (input: PharmaciesXml.Poikkeusaika) =
    let holidayName = Some(input.Otsikko)
    let openingHour = parseRegularDay input.Aukeaa.String input.Sulkeutuu.String null
    ServiceLocationJson.ServiceHour
      (
        additionalInformation = CommonParser.parseAdditionalInfo holidayName Defaults.textLengths.AdditionalInfo,
        isAlwaysOpen = false,
        isClosed = Option.isNone openingHour,
        isReservation = false,
        validForNow = false,
        validFrom = Some(DateTimeOffset(input.Pvm)),
        validTo = None,
        serviceHourType = Defaults.exceptionalHoursType,
        openingHour = itemToArray openingHour
      )
    
  let parseExceptionalHours (daysOfTheWeek: PharmaciesXml.Aukioloaika list) (holidays: PharmaciesXml.Poikkeusaika list) =
    let resultDays = daysOfTheWeek |> List.collect (fun item -> parseSpecialHours "Poikkeusjakso" item Defaults.exceptionalHoursType |> List.ofSeq)
    let resultHolidays = holidays |> List.map parseHoliday
    resultDays |> List.append resultHolidays
  
  let parseServiceHours (pharmacy: PharmaciesXml.Apteekki) =
    let (normalHours, otherHours) = List.partition
                                      (fun (item: PharmaciesXml.Aukioloaika) -> item.Luokittelu = "normaali")
                                      (pharmacy.Aukioloajat2.Aukioloaikas |> List.ofArray)
    let (overMidnightHours, exceptionalHours) = List.partition (fun (item: PharmaciesXml.Aukioloaika) -> item.Luokittelu = "päivystys") otherHours
    let overMidnightResult = overMidnightHours
                              |> List.collect (fun item -> parseSpecialHours "Päivystys" item Defaults.overMidnightHoursType |> List.ofSeq) 
    let exceptionalResult = pharmacy.Poikkeusajat.Poikkeusaikas
                            |> List.ofArray
                            |>  parseExceptionalHours exceptionalHours 
    
    normalHours
    |> List.collect (fun item -> parseNormalOpeningHour item |> List.ofSeq)
    |> List.append overMidnightResult
    |> List.append exceptionalResult
    |> List.toArray
    