namespace PTV.ExternalData.Pharmacy

open System
open System.Text
open System.Text.RegularExpressions

module Utils =
  let inline (|?) (a: 'a option) b =
    Option.defaultValue b a
    
  let log message (builder: StringBuilder) =
    builder.Append ("[" + DateTime.UtcNow.ToString() + "] " + message)
    |> ignore
    
  let itemToArray item =
    match item with
    | Some x -> [| x |]
    | None -> [||]

  let (|Regex|_|) pattern input =
    let m = Regex.Match(input, pattern, RegexOptions.IgnoreCase)
    if m.Success
    then Some m.Value
    else None
    
  let (|EmptyString|_|) text =
    if String.IsNullOrWhiteSpace text
    then None
    else Some text
    
  let (|LastIndex|_|) searchPattern (input: string) =
    let index = input.LastIndexOf(searchPattern, StringComparison.InvariantCulture)
    if index >= 0
    then Some index
    else None
    
  let weekdayToString (weekday: Weekday) =
    weekday.ToString()
    
  /// Splits a sequence into smaller batches of the same size. Last sequence may be smaller.
  let batch length (xs: seq<'T>) =
    let rec loop xs =
      [
        yield Seq.truncate length xs |> Seq.toList
        match Seq.length xs <= length with
        | false -> yield! loop (Seq.skip length xs)
        | true -> ()
      ]
    loop xs
    