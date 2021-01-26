namespace PTV.ExternalData.Pharmacy

open System
open System.IO
open System.Net.Http
open System.Text

module Importer =
  let private putConnectionRequests (channelIds: Guid[]) =
    let channelRelations = 
          channelIds
          |> Array.map (fun id ->  ConnectionJson.ChannelRelation
                                    (
                                      serviceChannelId = id.ToString(),
                                      contactDetails =
                                        ConnectionJson.ContactDetails
                                          (
                                            webPages = [|
                                              ConnectionJson.WebPage
                                                (
                                                  url = Defaults.pharmaciesHomePage,
                                                  value = Defaults.pharmaciesHopePageText,
                                                  language = Defaults.language
                                                )
                                            |],
                                            deleteAllWebPages = true
                                          )
                                    ))
    ConnectionJson.Service
      (
        deleteAllChannelRelations = true,
        channelRelations = channelRelations
      )
        
  let checkFiles serviceLocations eChannels =
    let locationLines = serviceLocations |> List.map (fun item -> item.ToString())
    let eLines = eChannels |> List.map (fun item -> item.ToString())
    File.WriteAllLines("XServiceLocations.json", locationLines)
    File.WriteAllLines("YEChannels.json", eLines)
    
  let import (secret: Secret) (enumCache: EnumCache) (builder: StringBuilder) =
    async {
      use client = new HttpClient()
      let! xml = client.GetStringAsync(secret.DownloadUrl)
                |> Async.AwaitTask
      let! token = OpenApiClient.authenticate secret builder
      let! existingChannels = OpenApiClient.getExistingChannels token secret builder
      let existingSourceIds = existingChannels
                             |> Array.map (fun item -> item.SourceId)
                             |> Array.choose id
      let existingIds = existingChannels
                       |> Array.filter (fun item -> item.ServiceChannelType.ToLowerInvariant() = "servicelocation")
                       |> Array.map (fun item -> item.Id)

      Utils.log ("Downloading and parsing pharmacies from " + secret.DownloadUrl) builder
      let pharmacies = PharmaciesXml.Parse(xml)
      let serviceLocations = ServiceLocationParser.parseServiceLocations secret enumCache pharmacies
      let (putLocationList, postLocationList) =
       serviceLocations |> List.partition (fun item -> Array.contains item.SourceId existingSourceIds)
       // E-channel specification is not finalized yet, so they will be imported later
//      let eChannels = EChannelParser.parseEChannels secret enumCache pharmacies
//      let (putEChannelList, postEChannelList) =
//       eChannels |> List.partition (fun item -> Array.contains item.SourceId existingSourceIds)
      Utils.log ("Parsed " + serviceLocations.Length.ToString() + " service locations.") builder
//      Utils.log ("Parsed " + serviceLocations.Length.ToString() + " service locations and " + eChannels.Length.ToString() + "e channels") builder
      // TODO: Just for debugging purposes
      //  checkFiles serviceLocations eChannels
      //  return "Done"
        
      do! putLocationList
        |> Seq.map (fun item -> OpenApiClient.putContent token secret.PutLocationUrl item.SourceId (item.ToString()) builder)
        |> Async.Sequential
        |> Async.Ignore
       
      do! postLocationList
        |> Seq.map (fun item -> OpenApiClient.postContent token secret.PostLocationUrl item.SourceId (item.ToString()) builder)
        |> Async.Sequential
        |> Async.Ignore
       
//      do! putEChannelList
//        |> Seq.map (fun item -> OpenApiClient.putContent token secret.PutEChannelUrl item.SourceId (item.ToString()) builder)
//        |> Async.Sequential
//        |> Async.Ignore
//       
//      do! postEChannelList
//        |> Seq.map (fun item -> OpenApiClient.postContent token secret.PostEChannelUrl item.SourceId (item.ToString()) builder)
//        |> Async.Sequential
//        |> Async.Ignore
       
      let connectionRequest = putConnectionRequests existingIds
      let connectionUrl = secret.PutConnectionUrl + secret.ServiceId
      do! OpenApiClient.putConnection token connectionUrl (connectionRequest.ToString()) builder
          |> Async.Ignore
         
      return builder.ToString()
    } |> Async.StartAsTask
