namespace PTV.ExternalData.Pharmacy

open System

module EChannelParser =
  let joinWithJa (input: string list) =
    if input |> List.isEmpty
    then  ""
    elif input.Length = 1
    then input.[0] + "."
    else
      let index = Math.Max(0, input.Length - 2)
      let prefix = String.Join(", ", input.[0..index])
      let complete = String.Join(" ja ", [ prefix; (input |> List.last) ])
      if complete |> String.IsNullOrWhiteSpace
      then complete
      else complete + "."
  
  let parseSummary (name: string) (serviceMethods: string option) =
    let suffix = (Option.defaultValue "" serviceMethods).Split(";", StringSplitOptions.RemoveEmptyEntries)
                  |> List.ofArray
                  |> List.map (fun item -> item.ToLowerInvariant())
                  |> joinWithJa
    name + " myy products " + suffix
    
  let parseDescription (summary: string) (deliveryMethods: string option) =
    let suffix = (Option.defaultValue "" deliveryMethods).Split(";")
                  |> List.ofArray
                  |> List.map (fun item -> item.ToLowerInvariant())
                  |> joinWithJa
    summary + " Verkkoapteekin toimitustavat ovat " + suffix
  
  let parseEChannel (secret: Secret) (enumCache: EnumCache) (input: PharmaciesXml.Apteekki) =
    if Option.isNone input.Verkkopalvelu || Option.isNone input.Verkkopalvelunmarkkinointinimi
    then None
    else
      let name = input.Nimi + " " + Defaults.eChannelSuffix
      let summary = parseSummary name input.ApteekinVerkkokaupanTuotevalikoima
      let description = parseDescription summary input.Erityispalvelut
      Some(EChannelJson.EChannel
                (
                  sourceId = CommonParser.buildEChannelSourceId input.Kelanumero,
                  serviceChannelDescriptions = [|
                    CommonParser.parseDescription description "Description" 2500;
                    CommonParser.parseDescription summary "Summary" 150
                  |],
                  organizationId = secret.OrganizationId,
                  areaType = Defaults.areaType,
                  serviceChannelNames = [| CommonParser.parseName name 100 |],
                  areas = [||],
                  signatureQuantity = None,
                  requiresSignature = false,
                  requiresAuthentication = true,
                  webPage = CommonParser.parseWebPages (Some name) CommonParser.formatEChannelWebPage,
                  attachments = [||],
                  supportPhones = CommonParser.parseSupportPhone input.Puhelin input.PuhelunHinta,
                  supportEmails = [||],
                  languages = CommonParser.parseLanguages input.Palvelukielet enumCache.LanguageCodes,
                  serviceHours = [||],
                  accessibilityClassification = [|
                    EChannelJson.AccessibilityClassification
                      (
                        accessibilityClassificationLevel = Defaults.accessibilityClassificationLevel,
                        wcagLevel = None,
                        accessibilityStatementWebPageName = None,
                        accessibilityStatementWebPage = None,
                        language = Defaults.language
                      )
                  |],
                  publishingStatus = Defaults.publishingStatus,
                  isVisibleForAll = true,
                  services = [| secret.ServiceId |],
                  deleteAllAttachments = true,
                  validFrom = None,
                  deleteAllServiceHours = true,
                  deleteAllSupportEmails = true,
                  deleteAllSupportPhones = true,
                  validTo = None
                ))
  
  let parseEChannels (secret: Secret) (enumCache: EnumCache) (pharmacies: PharmaciesXml.Apteekit) =
    pharmacies.Apteekkis
    |> Seq.map (parseEChannel secret enumCache)
    |> Seq.choose id
    |> Seq.toList