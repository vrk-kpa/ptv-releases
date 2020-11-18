namespace PTV.ExternalData.Pharmacy

open System.Text
open FSharp.Data
open PTV.ExternalData.Pharmacy

/// A module is a collection of functions. In C#, its closest equivalent is a static class.
/// This module contains a collection of requests to the PTV Open API.
module OpenApiClient =
  /// <summary>Asynchronously and recursively fetches and combines all pages of the paginated
  /// result of service locations for the given organization.</summary>
  /// <param name="token">Authentication token. Normally, it would not be needed
  /// for OUT API, but the source ID is displayed only for logged users.</param>
  /// <param name="baseUrl">Base url for getting one page.</param>
  /// <param name="acc">Accumulator for gathering all results.</param>
  /// <param name="page">The page of the paginated result that should be retrieved.</param>
  /// <returns>A list of all service channels of given organization.</returns>
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

  /// <summary>Asynchronously fetches all existing source IDs for service locations
  /// of given organization.
  ///
  /// Note, that F# is sometimes able to determine the type of an argument from the code.
  /// So, token does not need to be specified as (token: string). However, it cannot determine
  /// the type of secrets, therefore they need the type annotation explicitly.</summary>
  /// <param name="token">The authentication bearer token.</param>
  /// <param name="secrets">Collection of AWS secrets.</param>
  /// <returns>Array of existing source IDs as strings.</returns>
  let getExistingSourceIds token (secrets: SecretsJson.Secret) =
    async {
      Utils.log "3. Getting existing source IDs"
      // Create a base url for given organization.
      let url = secrets.PharmaGetLocationUrl + "?organizationId=" + secrets.PharmaOrganizationId
      // Asynchronously fetch all pages of the paginated result via a recursive function.
      // The parameters are: bearer token, base url, empty list accumulator and the starting page (1)
      let! allPages = getAllPages token url [||] 1
      // Returns all non-empty source IDs. The C# equivalent: allPages.Select(x => x.SourceId).Where(x => x != null)
      return allPages
             |> Array.map (fun item -> item.SourceId)
             |> Array.choose id
    }
    
  /// <summary>A special method for logging the results of POST and PUT requests to the Open API.
  /// This can be removed later, or replaced by a function which logs only errors.</summary>
  /// <param name="method">The HTTP method used in the request.</param>
  /// <param name="sourceId">Source ID of the pharmacy.</param>
  /// <param name="response">An FSharp.Data.HttpResponse object.</param>
  let logPutPostResponse method sourceId (response: HttpResponse) =
    Utils.log ("6. " + method + " location " + sourceId + " RESULT: " + response.StatusCode.ToString())
    if (response.StatusCode > 399)
    then Utils.log ("7. ERROR: " + response.Body.ToString())

  /// <summary>Updates an existing service location using the PUT method in Open API.</summary>
  /// <param name="token">Authentication bearer token.</param>
  /// <param name="secrets">AWS secrets object.</param>
  /// <param name="location">The service location that should be updated.</param>
  /// <returns>A tuple containing the source ID and response HTTP code.</returns>
  let putLocation token (secrets: SecretsJson.Secret) (location: ServiceLocationJson.ServiceLocation) =
    async {
      // Asynchronously send an HTTP request and await the response
      let! response =
        Http.AsyncRequest
          // Build the complete URL combining the base URL and source ID.
          // Note, that location.SourceId.Value usually not safe, because it may fail on a NullReference exception.
          // However, here we are sure that a SourceId is always present.
          ((secrets.PharmaPutLocationUrl + location.SourceId.Value),
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
           body = TextRequest(location.ToString()),
           // By default, HTTP status codes 4XX and 5XX throw an exception, which terminates the whole AWS lambda
           // function. We do not want the function to stop, just to log the errors and continue with the next
           // service location. Therefore we have to set silentHttpErrors to true.
           silentHttpErrors = true)

      logPutPostResponse "PUT" location.SourceId.Value response
      return (location.SourceId, response.StatusCode)
    }

  /// <summary>Creates a new service location using the POST method in Open API.</summary>
  /// <param name="token">Authentication bearer token.</param>
  /// <param name="secrets">AWS secrets object.</param>
  /// <param name="location">The service location that should be updated.</param>
  /// <returns>A tuple containing the source ID and response HTTP code.</returns>
  let postLocation token (secrets: SecretsJson.Secret) (location: ServiceLocationJson.ServiceLocation) =
    async {
      // Asynchronously send an HTTP request and await the response
      let! response =
        Http.AsyncRequest
          // For POST, we do not have to specify source ID in the URL, because it is part of the request body.
          (secrets.PharmaPostLocationUrl,
           // Set HTTP method to POST.
           httpMethod = HttpMethod.Post,
           // Same as for PUT. Authorization is set to bearer token, content type has to be application/json
           // with a UTF-16 (Unicode) charset.
           headers =
             [ HttpRequestHeaders.ContentTypeWithEncoding (HttpContentTypes.Json, Encoding.Unicode)
               HttpRequestHeaders.Authorization token ],
           // Serialized service location JSON.
           body = TextRequest(location.ToString()),
           // Silence the HTTP exceptions to continue the AWS lambda in execution even if an error occurs.
           silentHttpErrors = true)

      logPutPostResponse "POST" location.SourceId.Value response
      return (location.SourceId, response.StatusCode)
    }
    
  /// <summary>
  /// Creates a HTTP request for PAHA authentication.
  /// </summary>
  /// <param name="secrets">
  /// A list of secrets from AWS Secrets manager.
  /// </param>
  /// <returns>
  /// An HTTP request for PAHA environment.
  /// </returns>
  let private getPahaAuthRequest (secrets: SecretsJson.Secret) =
    let credentials = LoginRequestJson.Request ( username = secrets.PharmaApiUsername, password = secrets.PharmaApiPassword )
      
    let body = TextRequest(credentials.ToString())
      
    Http.AsyncRequestString
        (secrets.PharmaAuthUrl,
         httpMethod = HttpMethod.Post,
         headers = [ HttpRequestHeaders.ContentType HttpContentTypes.Json ],
         body = body)
    
  /// <summary>
  /// Creates a HTTP request for non-PAHA authentication.
  /// </summary>
  /// <param name="secrets">
  /// A list of secrets from AWS Secrets manager.
  /// </param>
  /// <returns>
  /// An HTTP request for non-PAHA environment.
  /// </returns>
  let private getLocalAuthRequest (secrets: SecretsJson.Secret) =
    let credentials =
      [ "username", secrets.PharmaApiUsername
        "password", secrets.PharmaApiPassword ]
      
    let body = FormValues(credentials)
      
    Http.AsyncRequestString
        (secrets.PharmaAuthUrl,
         httpMethod = HttpMethod.Post,
         headers = [ HttpRequestHeaders.ContentType HttpContentTypes.FormValues ],
         body = body)

  /// <summary>
  /// Posts given credentials to the authentication provider (PAHA or PTV STS)
  /// and returns obtained token.
  /// </summary>
  /// <param name="secrets">
  /// A list of secrets from AWS Secrets manager.
  /// </param>
  /// <returns>
  /// A string token, in the form "Bearer ...".
  /// </returns>
  let authenticate (secrets: SecretsJson.Secret) =
    async {
      Utils.log ("1. Authenticating user " + secrets.PharmaApiUsername + " for URL " + secrets.PharmaAuthUrl)

      // Asynchronously get the authentication response.
      let! response = if secrets.PharmaUsePaha |> bool.Parse
                      then getPahaAuthRequest secrets
                      else getLocalAuthRequest secrets
        

      Utils.log "2. Token received."
      // Parse the response.
      let token = LoginResponseJson.Parse(response)
      // Return bearer token.
      return "Bearer " + token.ServiceToken
    }
