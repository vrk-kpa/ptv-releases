namespace PTV.ExternalData.Pharmacy

open System
open System.Collections.Generic
open PTV.ExternalData.Pharmacy.Utils

module ServiceLocationParser =    
  let parseAreaType areas =
    match areas with
    | Some _ -> Defaults.limitedAreaType
    | None -> Defaults.areaType
    
  let parseServiceLocation (secret: Secret) (enumCache: EnumCache) (input: PharmaciesXml.Apteekki) =
    let areas = if enumCache.PostOfficeMunicipalities.ContainsKey input.Kayntiosoite.Toimipaikka
                then
                  let areaCodes = enumCache.PostOfficeMunicipalities.[input.Kayntiosoite.Toimipaikka]
                                  |> Seq.distinct
                                  |> Array.ofSeq
                  Some(ServiceLocationJson.Area(``type`` = Defaults.areaTypeMunicipality, areaCodes = areaCodes))
                else None
                
    ServiceLocationJson.ServiceLocation
      (
        sourceId = CommonParser.buildLocationSourceId input.Kelanumero,
        localizedText = None,
        nameOrDescription = None,
        serviceChannelNames = [| CommonParser.parseNameOrDescription input.Nimi "Name" 100 |],
        serviceChannelDescriptions = [|
          CommonParser.parseNameOrDescription (input.Nimi + " " + Defaults.description) "Description" 2500;
          CommonParser.parseNameOrDescription (input.Nimi + " " + Defaults.summary) "Summary" 150
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

