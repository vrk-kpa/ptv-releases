namespace PTV.ExternalData.Pharmacy

open System.Text.RegularExpressions
open PTV.ExternalData.Pharmacy.Utils

module AddressParser =
  let formatStreetAddressText streetName =
    let m = Regex.Match(streetName,  @"\(.*\)")
    if m.Success
    then streetName.Substring(0, m.Index) + ", " + streetName.Substring(m.Index + 1, m.Length - 2)
    else streetName
    
  let parseAdditionalInfo (streetName: string) splitIndex =
    let part1 = streetName.Substring(0, splitIndex).Trim()
    let part2 = streetName.Substring(splitIndex + 1).Trim()
    if List.exists (fun (item: string) -> part1.ToLowerInvariant().Contains(item)) Defaults.addressShopNames
    then (part2, Some part1)
    else (part1, Some part2)
    
  let handleDualNames (streetAddress: string, additionalInfo: string option) =
    let m = Regex.Match(streetAddress, @"\s+\-\s+")
    if m.Success
    then
      let part1 = streetAddress.Substring(0, m.Index)
      let part2 = streetAddress.Substring(m.Index + m.Length)
      if List.exists (fun (item: string) -> part1.ToLowerInvariant().Contains(item)) Defaults.swedishStreetNames
      then (part2, additionalInfo)
      else (part1, additionalInfo)
    else (streetAddress, additionalInfo)
    
  let getAdditionalInfo streetName =
    match streetName with
     | LastIndex "," index -> parseAdditionalInfo streetName index
     | LastIndex "/" index -> parseAdditionalInfo streetName index
     | _ -> (streetName, None)
     
  let parseNameAndNumber (streetAndNumber: string) =
    let index = streetAndNumber.IndexOfAny("0123456789,".ToCharArray())
    if index < 0
    then (streetAndNumber.Trim(), "")
    else (streetAndNumber.Substring(0, index).Trim(), streetAndNumber.Substring(index).Trim())
  
  let parseAddress (input: PharmaciesXml.Kayntiosoite) =
    let formatted = formatStreetAddressText input.Katuosoite
    let (streetAddress, additionalInfo) = getAdditionalInfo formatted |> handleDualNames
    let (streetName, streetNumber) = parseNameAndNumber streetAddress
    
    ServiceLocationJson.Address
      (
        ``type`` = Defaults.addressType,
        subType = Defaults.addressSubtype,
        postOfficeBoxAddress = None,
        streetAddress = ServiceLocationJson.StreetAddress
                        (
                          street = [| CommonParser.createLocalizedText streetName 100 |],
                          streetNumber = streetNumber,
                          postalCode = CommonParser.removeWhitespaces (input.Postinumero.String |? ""),
                          municipality = "",
                          additionalInformation = CommonParser.parseAdditionalInfo additionalInfo 150,
                          longitude = "",
                          latitude = ""
                        ),
          otherAddress = None,
          locationAbroad = [||],
          country = None
      )
      