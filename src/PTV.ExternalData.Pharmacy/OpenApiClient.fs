namespace PTV.ExternalData.Pharmacy

open System.Text
open FSharp.Data

module OpenApiClient =
  let rec getAllPages token baseUrl (acc: OrganizationServiceChannelJson.ItemList []) (page: int) =
    async {
      // Build the URL for given page.
      let pageUrl = baseUrl + "&page=" + page.ToString()
      // Await the response.
      let! response = Http.AsyncRequestString
                        (pageUrl,
                         headers =
                           [ HttpRequestHeaders.ContentType HttpContentTypes.Json
                             HttpRequestHeaders.Authorization token ])
      // Parse the content of the response.
      let content = OrganizationServiceChannelJson.Parse(response)
      // Add new results to existing accumulator.
      let mergedArray = Array.concat [ content.ItemList; acc ]

      // If there are still more pages...
      if content.PageNumber < content.PageCount
      // ...then recursively call the same asynchronous function for the next page...
      then return! getAllPages token baseUrl mergedArray (page + 1)
      // ...otherwise return the merged result.
      else return mergedArray
    }
    
  let getExistingChannels token (secret: Secret) (builder: StringBuilder) =
    // Create a base url for given organization.
    let url = secret.GetChannelUrl + "?organizationId=" + secret.OrganizationId
    try
      async {
        // Asynchronously fetch all pages of the paginated result via a recursive function.
        // The parameters are: bearer token, base url, empty list accumulator and the starting page (1)
        let! allPages = getAllPages token url [||] 1
        // Returns all non-empty source IDs. The C# equivalent: allPages.Select(x => x.SourceId).Where(x => x != null)
        return allPages
      }
    with
    | ex ->
      Utils.log ("Failed to get existing channels for URL " + url) builder
      reraise()
    
  let private getPahaAuthRequest (secret: Secret) =
    let credentials = LoginRequestJson.Request ( username = secret.ApiUserName, password = secret.ApiPassword )
      
    let body = TextRequest(credentials.ToString())
      
    Http.AsyncRequestString
        (secret.AuthenticationUrl,
         httpMethod = HttpMethod.Post,
         headers = [ HttpRequestHeaders.ContentType HttpContentTypes.Json ],
         body = body)
    
  let private getLocalAuthRequest (secret: Secret) =
    let credentials =
      [ "username", secret.ApiUserName
        "password", secret.ApiPassword ]
      
    let body = FormValues(credentials)
      
    Http.AsyncRequestString
        (secret.AuthenticationUrl,
         httpMethod = HttpMethod.Post,
         headers = [ HttpRequestHeaders.ContentType HttpContentTypes.FormValues ],
         body = body)
        
  let authenticate (secret: Secret) (builder: StringBuilder) =    
    try
      async {
        // Asynchronously get the authentication response.
        let! response = if secret.UsePaha
                        then getPahaAuthRequest secret
                        else getLocalAuthRequest secret
          
        // Parse the response.
        let token = LoginResponseJson.Parse(response)
        // Return bearer token.
        return "Bearer " + token.ServiceToken
      }
    with
    | ex ->
      Utils.log ("Authentication failed for user " + secret.ApiUserName + " URL " + secret.AuthenticationUrl + " PAHA " + secret.UsePaha.ToString()) builder
      Utils.log ex.Message builder
      reraise()
    
  let logPutPostResponse sourceId (response: HttpResponse) (builder: StringBuilder) =
    if (response.StatusCode > 399)
    then Utils.log (sourceId + " ERROR: " + response.Body.ToString()) builder
    
  let putContent token url sourceId content builder =
    async {
      try
      // Asynchronously send an HTTP request and await the response
        let! response =
          Http.AsyncRequest
            // Build the complete URL combining the base URL and source ID.
            // Note, that location.SourceId.Value usually not safe, because it may fail on a NullReference exception.
            // However, here we are sure that a SourceId is always present.
            ((url + sourceId),
             // Specify the method as PUT.
             httpMethod = HttpMethod.Put,
             // In headers, set the Authorization value to the provided bearer token.
             // Note, that the content type has to be set to application/json and charset UTF-16 (Unicode).
             // The input XML file is in UTF-16, but the default charset for HTTP is UTF-8. If we do not
             // specify the charset explicitly, we get a server error.
             headers =
               [ HttpRequestHeaders.ContentTypeWithEncoding (HttpContentTypes.Json, Encoding.Unicode)
                 HttpRequestHeaders.Authorization token ],
             // Set the body to the serialized JSON object.
             body = TextRequest(content),
             // By default, HTTP status codes 4XX and 5XX throw an exception, which terminates the whole AWS lambda
             // function. We do not want the function to stop, just to log the errors and continue with the next
             // service location. Therefore we have to set silentHttpErrors to true.
             silentHttpErrors = true)

        logPutPostResponse sourceId response builder
        return (sourceId, response.StatusCode)
      with
      | ex ->
        Utils.log (sourceId + " PUT EXCEPTION: " + ex.Message) builder
        return (sourceId, -1)
    }
    
  
  let postContent token url sourceId content builder =
    async {
      // Asynchronously send an HTTP request and await the response
      try
        let! response =
          Http.AsyncRequest
            // For POST, we do not have to specify source ID in the URL, because it is part of the request body.
            (url,
             // Set HTTP method to POST.
             httpMethod = HttpMethod.Post,
             // Same as for PUT. Authorization is set to bearer token, content type has to be application/json
             // with a UTF-16 (Unicode) charset.
             headers =
               [ HttpRequestHeaders.ContentTypeWithEncoding (HttpContentTypes.Json, Encoding.Unicode)
                 HttpRequestHeaders.Authorization token ],
             // Serialized service location JSON.
             body = TextRequest(content),
             // Silence the HTTP exceptions to continue the AWS lambda in execution even if an error occurs.
             silentHttpErrors = true)

        logPutPostResponse sourceId response builder
        return (sourceId, response.StatusCode)
      with
      | ex ->
        Utils.log (sourceId + " POST EXCEPTION: " + ex.Message) builder
        return (sourceId, -1)
    }
    
  let postConnection token url content builder =
    async {
      try
        // Asynchronously send an HTTP request and await the response
        let! response =
          Http.AsyncRequest
            // Build the complete URL combining the base URL and source ID.
            // Note, that location.SourceId.Value usually not safe, because it may fail on a NullReference exception.
            // However, here we are sure that a SourceId is always present.
            (url,
             // Specify the method as POST.
             httpMethod = HttpMethod.Post,
             // In headers, set the Authorization value to the provided bearer token.
             // Note, that the content type has to be set to application/json and charset UTF-16 (Unicode).
             // The input XML file is in UTF-16, but the default charset for HTTP is UTF-8. If we do not
             // specify the charset explicitly, we get a server error.
             headers =
               [ HttpRequestHeaders.ContentTypeWithEncoding (HttpContentTypes.Json, Encoding.Unicode)
                 HttpRequestHeaders.Authorization token ],
             // Set the body to the serialized JSON object.
             body = TextRequest(content),
             // By default, HTTP status codes 4XX and 5XX throw an exception, which terminates the whole AWS lambda
             // function. We do not want the function to stop, just to log the errors and continue with the next
             // service location. Therefore we have to set silentHttpErrors to true.
             silentHttpErrors = true)

        logPutPostResponse url response builder
        return response.StatusCode
      with
      | ex ->
        Utils.log ("Connection EXCEPTION: " + ex.Message) builder
        return -1
    }
    