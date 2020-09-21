namespace PTV.ExternalData.Pharmacy

open Amazon.Lambda.Core
open Amazon
open PTV.ExternalData.Pharmacy.Utils
open Amazon.S3
open Amazon.S3.Util
open System.IO
open Amazon.S3.Model
open Amazon.SecretsManager.Extensions.Caching

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly:LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

/// The main function, equivalent of a C# Function class
type Function() =

  /// <summary>Asynchronously fetches and parses an XML file from the S3 bucket.
  ///
  /// In F#, the let keyword can be used to specify both a variable and a new function.
  /// Here, the let keyword introduces a function called processFile, which takes 3 parameters,
  /// then performs some asynchronous operations and returns a parsed list of ServiceLocations.
  /// The same function in F# would have the following signature:
  ///   public async Task<List<ServiceLocation>> ProcessFile(AmazonS3Client client, SecretJson.Secret secrets, S3EventNotificationRecord fileEvent)
  /// </summary>
  /// <param name="message"></param>
  /// <returns></returns>
  let processFile
      (client: AmazonS3Client)
      (secrets: SecretsJson.Secret)
      (fileEvent: S3EventNotification.S3EventNotificationRecord)
    =
    // This marks that a part of the function body contains asynchronous workflow
    async {
      // Get the bucket name and file name from the file event.
      let bucketName = fileEvent.S3.Bucket.Name
      let fileName = fileEvent.S3.Object.Key

      log ("4. Processing file " + bucketName + " > " + fileName)

      // Create a new request to read the specified file in the bucket.
      let request = GetObjectRequest(BucketName = bucketName, Key = fileName)

      // Asynchronously get the response, extract the response stream and
      // pass it into a stream reader. Because .GetObjectAsync(request) and .ReadToEndAsync()
      // are C# methods returning Task<T>, we have to convert them into Async<T> using Async.AwaitTask.
      use! response = client.GetObjectAsync(request) |> Async.AwaitTask
      use responseStream = response.ResponseStream
      use reader = new StreamReader(responseStream)
      // Asynchronously read the content of the file
      let! content = reader.ReadToEndAsync() |> Async.AwaitTask

      // Return a list of parsed pharmacies
      return PharmacyParser.parsePharmacies secrets content
    }

  /// <summary>
  /// This is the main entry point of the whole lambda function (as specified in
  /// /terraform/modules/pharmacies-lambda/variables.tf, variable "handler").
  ///
  /// In F#, the order of the files in the ".fsproj" file matters. If you have files A and B
  /// and a function in file A calls a function from file B, then in fsproj file B has to be placed
  /// earlier than file A, so that the called function in file B is compiled earlier than
  /// the caller function in file A. Otherwise, you get a compiler error that the function does
  /// not exist. Therefore, the Function.fs file and the __.FunctionHandler have to placed as the last
  /// in order. Note, that JetBrains Rider is able to show the files in the order specified in the
  /// .fsproj file. Visual Studio Code, however, displays them in the alphabetical order.
  /// </summary>
  /// 
  /// <param name="input">A notification from the S3 bucket that a new file was uploaded.
  /// The notification trigger is specified in the terraform module "pharmacies-lambda"
  /// (search for "s3:ObjectCreated:*").</param>
  /// 
  /// <param name="context">Lambda function context. We don't use it, so it can be ignored.</param>
  /// 
  /// <returns>An asynchronous C# Task of string. We don't really need to return anything,
  /// but the AWS documentation specifies that it does not support async void. Since the F#
  /// Async<unit> can be sometimes translated to async Task and sometimes to async void, it
  /// is safer to return some value.</returns>
  member __.FunctionHandler (input: S3EventNotification) (_: ILambdaContext): System.Threading.Tasks.Task<string> =
    log "0. Function handler started."

    // We begin our asynchronous computation by specifying the async body. Note, that Async<'a> in F# behaves
    // a little different from async Task<T> in C#. In C#, the computation begins immediately and ends sometime
    // in the future (you may or may not await it). In F#, the computation does not begin until you explicitly say so.
    async {
      // Get the Amazon S3 client, to access S3 objects. Normally, we would have to specify IAM credentials
      // for the lambda to access S3. However, this can be done also in terraform, so the client code in F#
      // can be made simpler.
      // The "use" keyword specifies that we are working with an IDisposable resource. It is like "using" in C#,
      // but F# automatically takes care of disposing the object when it goes out of scope
      // (e.g.: at the end of a function).
      use client = new AmazonS3Client(RegionEndpoint.EUWest1)
      // Get the secrets cache, so we can access values from AWS SecretsManager. Again, terraform is taking care
      // of granting access to the specific secrets value.
      use secretsCache = new SecretsManagerCache()

      // Asynchronously get a secret of the specified key and return as a string. Because SecretsManagerCache is
      // a C# class, it works with Task<string>. Therefore, in F# we have to convert it to Async<string>. This is
      // done by the Async.AwaitTask function. The pipe operator (|>) simply means that you should take the result
      // of the function to the left and pass it to the function to the right. You could write the same code as:
      // Async.AwaitTask(secretsCache.GetSecretString(Defaults.secretKey))
      // The "let" keyword is similar to "var" in C#. "let!" means that we are awaiting an asynchronous result.
      // In C#,
      // let! secretsString = ...
      // would be
      // var secretsString = await ...
      let! secretsString = secretsCache.GetSecretString Defaults.secretKey |> Async.AwaitTask
      
      // Now we are getting to the interesting stuff. F# has an awesome library called FSharp.Data. This library
      // is able to dynamically create types from a JSON, XML or CSV resource. E.g.: the SecretsJson type is
      // created in the Types.fs file. We provide the FSharp.Data.JsonProvider with a sample json file. This
      // generates the SecretsJson type. Then, we get the actual json with the secret values from
      // the SecretsManagerCache. We let the SecretsJson parse the obtained secrets json string and we end up with
      // a fully statically typed object.
      let secrets = SecretsJson.Parse(secretsString)
      
      // Asynchronously get PAHA token from the OpenApiClient (in OpenApiClient.fs). We pass the secrets object as
      // an argument. Note, that in F# you don't have to use parentheses around function arguments. This is because
      // F# supports partial function calls, which then allows function composition. Here is an example:
      //   let add x y = x + y
      // ...is a function which takes two parameters (x, y), adds them together and returns the result.
      // ...in C#, add would have this signature: int Add(int x, int y)
      //   let three = add 1 2
      // ..."three" is a variable which stores the result of the function add 1 2 (i.e.: the value 3).
      // ...in C#, three would have this signature: int three
      //   let increment = add 1
      // ...if we pass only one argument, we essentially create a new function.
      // ..."increment" is a function which now takes only one parameter and always adds to it 1.
      // ...in C#, increment would have this signature: int Increment(int y)
      let! token = OpenApiClient.authenticate secrets
      
      // Asynchronously get existing source IDs from Open API. "token" and "secrets" are passed as parameters.
      let! existingIds = OpenApiClient.getExistingSourceIds token secrets
      
      log ("X. existing source IDs: " + System.String.Join(", ", existingIds))

      // For each file that was uploaded in the S3 notification...
      for record in input.Records do
        // Asynchronously process the file and convert it into a list of ServiceLocations
        let! json = processFile client secrets record
        // Split the list into two parts, based on the list of existing source IDs. If a pharmacy already exists,
        // it will be updated. If it is new, it will be created. Note, that we take the existing json and pipe (|>)
        // it into a partial function (List.partition) which takes two arguments: a function for splitting the list
        // and the original list. (fun item -> ...) is a anonymous function (similar to C# lambdas).
        // The whole call could be rewritten as this:
        // let (a, b) = List.partition (fun (item: ServiceLocationJson.ServiceLocation) -> Array.contains item.SourceId.Value existingIds) (json)
        let (putList, postList) =
          json |> List.partition (fun item -> Array.contains item.SourceId.Value existingIds)

        log ("5. Items to put: " + putList.Length.ToString() + " and post: " + postList.Length.ToString())

        // If the secrets tell us that we should also update existing content, we loop through the putList
        if secrets.PharmaUpdate |> bool.Parse then
          // do! means that we should asynchronously perform an operation, but we do not await any result.
          // It is similar to await Task. 
          do! putList
              // We take the putList and pass it into a function which creates asynchronous calls.
              |> Seq.map (OpenApiClient.putLocation token secrets)
              // We split it into batches of 15 calls to prevent too many parallel requests on Open API.
              |> Utils.batch 15
              // The requests in each batch can be called in parallel.
              |> Seq.map (fun batch -> batch |> Async.Parallel)
              // However, each batch should be called sequentially.
              |> Async.Sequential
              // We do not await any result, so we simply ignore it.
              |> Async.Ignore

        // Now, we do the same with the postList. In C#, the code would be like this:
        // var asyncRequests = postList.Select(pharmaJson => await OpenApiClient.PostLocation(token, secrets, pharmaJson));
        // var batches = asyncRequests.Batch(15);
        // foreach (var batch in batches)
        // {
        //     await Task.WhenAll(batch);
        // }
        do! postList
            |> Seq.map (OpenApiClient.postLocation token secrets)
            |> Utils.batch 15
            |> Seq.map (fun batch -> batch |> Async.Parallel)
            |> Async.Sequential
            |> Async.Ignore

      log "8. Done"
      // When everything is finished, we return a string. However, we could return also differnt values.
      return "Done"
    }
    // Because AWS Lambda awaits a Task<T> and our async body in an Async<T>, we have to convert it
    // into a Task using the Async.StartAsTask method.
    |> Async.StartAsTask
