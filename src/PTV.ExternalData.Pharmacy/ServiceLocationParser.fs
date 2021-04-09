namespace PTV.ExternalData.Pharmacy

open System
open PTV.ExternalData.Pharmacy.Utils

module ServiceLocationParser =    
  let parseAreaType areas =
    match areas with
    | Some _ -> Defaults.limitedAreaType
    | None -> Defaults.areaType
    
  let getPostOfficeName postOffice =
    if String.IsNullOrWhiteSpace postOffice
    then ""
    else
      let split = postOffice.Split("-", StringSplitOptions.RemoveEmptyEntries)
      if split |> Array.isEmpty
      then ""
      else split.[0].Trim().ToUpperInvariant()
    
  let parseServiceLocation (secret: Secret) (enumCache: EnumCache) (input: PharmaciesXml.Apteekki) =
    let postOfficeName = getPostOfficeName input.Kayntiosoite.Toimipaikka                           
    let areas = if enumCache.PostOfficeMunicipalities.ContainsKey postOfficeName
                then
                  let areaCodes = enumCache.PostOfficeMunicipalities.[postOfficeName]
                                  |> Seq.distinct
                                  |> Array.ofSeq
                  Some(ServiceLocationJson.Area(``type`` = Defaults.areaTypeMunicipality, areaCodes = areaCodes))
                else None
                
    ServiceLocationJson.ServiceLocation
      (
        sourceId = CommonParser.buildLocationSourceId input.Kelanumero,
        localizedText = None,
        nameOrDescription = None,
        serviceChannelNames = [| CommonParser.parseNameOrDescription input.Nimi "Name" Defaults.textLengths.Name |],
        serviceChannelDescriptions = [|
          CommonParser.parseNameOrDescription (input.Nimi + " " + Defaults.description) "Description" Defaults.textLengths.Description;
          CommonParser.parseNameOrDescription (input.Nimi + " " + Defaults.summary) "Summary" Defaults.textLengths.Summary
        |],
        organizationId = secret.OrganizationId,
        displayNameType = [| ServiceLocationJson.DisplayNameType(``type`` = "Name", language = Defaults.language) |],
        areaType = parseAreaType areas,
        areas = itemToArray areas,
        emails = [||],
        faxNumbers = (CommonParser.parseFaxNumber input.Faksi |> itemToArray),
        phoneNumbers = CommonParser.parsePhoneNumber input.Puhelin input.PuhelunHinta,
        languages = CommonParser.parseLanguages input.Palvelukielet enumCache.LanguageCodes,
        webPages = CommonParser.parseWebPages input.Kotisivut CommonParser.formatLocationWebPage,
        addresses = [| AddressParser.parseAddress input.Kayntiosoite |],
        serviceHours = OpeningHoursParser.parseServiceHours input,
        deleteAllEmails = true,
        deleteAllPhoneNumbers = true,
        deleteAllFaxNumbers = true,
        deleteOid = false,
        publishingStatus = Defaults.publishingStatus,
        isVisibleForAll = true,
        services = [| secret.ServiceId |],
        deleteAllWebPages = true,
        deleteAllServiceHours = true,
        oid = None,
        validFrom = None,
        validTo = None
      )
      
  let parseServiceLocations (secret: Secret) (enumCache: EnumCache) (pharmacies: PharmaciesXml.Apteekit) =
    // C# equivalent: pharmacies.Apteekkis.Select(x => parsePharmacy(secrets, x)).ToList()
    pharmacies.Apteekkis
    |> Seq.map (parseServiceLocation secret enumCache)
    |> Seq.toList
