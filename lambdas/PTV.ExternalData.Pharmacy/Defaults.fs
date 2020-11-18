namespace PTV.ExternalData.Pharmacy

// This file contains type definitions used in the rest of the AWS lambda function.
// If you want to know how the F# code works, start in the Function.fs file.

/// This module contains definitions of default values for required fields in Open API.
module Defaults =
  /// Default address type.
  let addressLocation = "Location"
  /// Default address subtype.
  let addressSingle = "Single"
  /// Default area type.
  let areaType = "Nationwide"
  /// Default description.
  let description = "myy sekä reseptilääkkeitä että itsehoitolääkkeitä, joiden ostamiseen et tarvitse lääkärin lääkemääräystä. Lisäksi apteekki myy lääkkeiden käyttöön tarvittavia välineitä, sidetarpeita, lääkinnällisiä laitteita, ravintolisiä ja kosmetiikkaa. Saat apteekista myös lääkeneuvontaa sekä opastusta lääkkeiden ja muiden apteekissa myytävien tuotteiden käytössä. Lisäksi saat neuvontaa sairauksien lääkkeettömään hoitoon ja terveyden edistämiseen.\nApteekkiin voit palauttaa käyttämättömäksi jääneet ja vanhentuneet lääkkeet."
  /// Default language.
  let language = "fi"
  /// Default type of opening hours.
  let openingHoursType = "DaysOfTheWeek"
  /// Chargeable phone type.
  let phoneCharged = "Chargeable"
  /// Phone type free of charge.
  let phoneFree = "FreeOfCharge"
  /// Default phone or fax prefix.
  let prefix = "+358"
  /// Default publishing status.
  let publishingStatus = "Published"
  /// Key under which the AWS SecretsManager stores secrets for this lambda function.
  let secretKey = "PharmacyLambda"
  /// Source ID prefix to avoid source ID conflicts.
  let sourceIdPrefix = "pharma-"
  /// Default summary.
  let summary = "myy reseptilääkkeitä, itsehoitolääkkeitä, lääkkeiden käyttöön tarvittavia välineitä ja sidetarpeita."
