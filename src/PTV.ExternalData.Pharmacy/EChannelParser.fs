namespace PTV.ExternalData.Pharmacy

module EChannelParser =  
  let parseEChannel (secret: Secret) (enumCache: EnumCache) (input: PharmaciesXml.Apteekki) =
    if Option.isNone input.Verkkopalvelu || Option.isNone input.Verkkopalvelunmarkkinointinimi
    then None
    else
      let name = input.Nimi + " " + Defaults.eChannelSuffix
      
      let mutable summary = name + " " + Defaults.eChannelSummary
      if summary.Length > Defaults.textLengths.Summary
      then summary <- name + " " + Defaults.eChannelShortSummary
      
      let description = name + " " + Defaults.eChannelDescription
      Some(EChannelJson.EChannel
                (
                  sourceId = CommonParser.buildEChannelSourceId input.Kelanumero,
                  serviceChannelDescriptions = [|
                    CommonParser.parseDescription description "Description" Defaults.textLengths.Description
                    CommonParser.parseDescription summary "Summary" Defaults.textLengths.Summary
                  |],
                  organizationId = secret.OrganizationId,
                  areaType = Defaults.areaType,
                  serviceChannelNames = [| CommonParser.parseName name Defaults.textLengths.Name |],
                  areas = [||],
                  signatureQuantity = None,
                  requiresSignature = false,
                  requiresAuthentication = true,
                  webPage = CommonParser.parseWebPages input.Verkkopalvelu CommonParser.formatEChannelWebPage,
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
    