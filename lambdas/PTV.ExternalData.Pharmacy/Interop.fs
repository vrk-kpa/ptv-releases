namespace PTV.ExternalData.Pharmacy

module Interop =
   let GetDefaultSecret orgId serviceId =
    SecretsJson.Secret
      (
        pharmaGetLocationUrl = "",
        pharmaAuthUrl = "",
        pharmaPutLocationUrl = "",
        pharmaPostLocationUrl = "",
        pharmaApiUsername = "",
        pharmaApiPassword = "",
        pharmaOrganizationId = orgId,
        pharmaServiceId = serviceId,
        pharmaUpdate = "true",
        pharmaUsePaha = "false"
      )

