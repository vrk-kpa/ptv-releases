namespace PTV.ExternalData.Pharmacy

open System
open System.IO
open System.Net.Http
open System.Text

module Importer =    
  let createConnectionRequest (serviceId: string) (channelId: Guid) (channelType: string) =
    let contactDetails =
      if channelType.ToLowerInvariant() = "servicelocation"
      then ConnectionJson.ContactDetails
            (
              webPages = [|
                    ConnectionJson.WebPage
                      (
                        url = Defaults.pharmaciesHomePage,
                        value = Defaults.pharmaciesHomePageText,
                        language = Defaults.language
                      )
                  |]
            ) |> Some
      else None
    
    ConnectionJson.Connection
      (
        serviceId = serviceId,
        serviceChannelId = channelId.ToString(),
        contactDetails = contactDetails
      )
        
  let checkFiles serviceLocations eChannels archiveIds (connectionRequests: ConnectionJson.Connection list list) =
    let locationLines = serviceLocations |> List.map (fun item -> item.ToString())
    let eLines = eChannels |> List.map (fun item -> item.ToString())
    let connectionLines = connectionRequests |> List.map (fun item ->
      "[" + String.Join (", ", item |> List.map (fun subitem -> subitem.ToString())) + "]"
      )
    File.WriteAllLines("XServiceLocations.json", locationLines)
    File.WriteAllLines("YEChannels.json", eLines)
    File.WriteAllLines("ZToBeDeleted.txt", archiveIds)
    File.WriteAllLines("WConnections.json", connectionLines)
    
  let getSourceIdsForArchiving (existingIds: string []) (serviceLocations: ServiceLocationJson.ServiceLocation list) (eChannels: EChannelJson.EChannel list) =
    let toBeUpdated = serviceLocations
                      |> List.map (fun item -> item.SourceId)
                      |> List.append (eChannels |> List.map (fun item -> item.SourceId))
                      
    existingIds
    |> Array.filter (fun id -> toBeUpdated |> List.contains id |> not)
    |> Array.map (fun id -> ArchiveJson.Request( sourceId = id, publishingStatus = "Deleted" ))
    |> Array.partition (fun item -> item.SourceId.StartsWith(Defaults.eChannelIdPrefix))
    
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

      Utils.log ("Downloading and parsing pharmacies from " + secret.DownloadUrl) builder
      let pharmacies = PharmaciesXml.Parse(xml)
      
      let serviceLocations = ServiceLocationParser.parseServiceLocations secret enumCache pharmacies
      let (putLocationList, postLocationList) =
       serviceLocations |> List.partition (fun item -> Array.contains item.SourceId existingSourceIds)
       
      let eChannels = EChannelParser.parseEChannels secret enumCache pharmacies
      let (putEChannelList, postEChannelList) =
       eChannels |> List.partition (fun item -> Array.contains item.SourceId existingSourceIds)
       
      let (archivedEChannels, archivedLocations) = getSourceIdsForArchiving existingSourceIds serviceLocations eChannels
      Utils.log ("Parsed " + serviceLocations.Length.ToString() + " service locations and " + eChannels.Length.ToString() + "e channels") builder
      // TODO: Just for debugging purposes
//      let connectionRequests = existingChannels
//                                |> Array.filter (fun item -> item.Services |> Array.isEmpty)
//                                |> Array.map (fun item -> createConnectionRequest secret.ServiceId item.Id item.ServiceChannelType)
//                                |> Utils.batch 10
//      let archiveIds = archivedEChannels
//                      |> Array.map (fun item -> item.SourceId)
//                      |> Array.append (archivedLocations |> Array.map (fun item -> item.SourceId))
//      checkFiles serviceLocations eChannels archiveIds connectionRequests
//      return "Done"
        
      do! putLocationList
        |> Seq.map (fun item -> OpenApiClient.putContent token secret.PutLocationUrl item.SourceId (item.ToString()) builder)
        |> Async.Sequential
        |> Async.Ignore
       
      do! postLocationList
        |> Seq.map (fun item -> OpenApiClient.postContent token secret.PostLocationUrl item.SourceId (item.ToString()) builder)
        |> Async.Sequential
        |> Async.Ignore
        
      do! archivedLocations
        |> Array.map (fun item -> OpenApiClient.putContent token secret.PutLocationUrl item.SourceId (item.ToString()) builder)
        |> Async.Sequential
        |> Async.Ignore
       
      do! putEChannelList
        |> Seq.map (fun item -> OpenApiClient.putContent token secret.PutEChannelUrl item.SourceId (item.ToString()) builder)
        |> Async.Sequential
        |> Async.Ignore
       
      do! postEChannelList
        |> Seq.map (fun item -> OpenApiClient.postContent token secret.PostEChannelUrl item.SourceId (item.ToString()) builder)
        |> Async.Sequential
        |> Async.Ignore
        
      do! archivedEChannels
        |> Array.map (fun item -> OpenApiClient.putContent token secret.PutEChannelUrl item.SourceId (item.ToString()) builder)
        |> Async.Sequential
        |> Async.Ignore
        
      do! existingChannels
          |> Seq.filter (fun item -> item.Services |> Array.isEmpty)
          |> Seq.map (fun item -> createConnectionRequest secret.ServiceId item.Id item.ServiceChannelType)
          |> Utils.batch 10
          |> Seq.map (fun item -> "[" + String.Join (", ", item |> List.map (fun subItem -> subItem.ToString())) + "]")
          |> Seq.map (fun request -> OpenApiClient.postConnection token secret.PostConnectionUrl request builder)
          |> Async.Sequential
          |> Async.Ignore
         
      return builder.ToString()
    } |> Async.StartAsTask
    