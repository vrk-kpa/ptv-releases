namespace PTV.ExternalData.Pharmacy

open System
open Microsoft.FSharp.Reflection
open System.Text.RegularExpressions

/// A module of various utility functions.
module Utils =
  /// <summary>
  /// Prints given message to the standard output, adding a UTC timestamp.
  /// AWS collects all the printed messages and stores them in the log stream.
  /// </summary>
  /// <param name="message">
  /// Given string message
  /// </param>
  let log message =
    printfn "%s" ("[" + DateTime.UtcNow.ToString() + "] " + message)

  /// <summary>A shorthand operator for Option.defaultValue a b.
  /// In C#, this would be the null coalescence operator (??).</summary>
  /// <param name="a">And Option<'a> value. In C#, this would be any nullable type.</param>
  /// <param name="b">An 'a value. Any default, non-option value that is returned if a is None.</param>
  /// <returns>The value of a, if a is Some, otherwise b.</returns>
  let inline (|?) (a: 'a option) b =
    Option.defaultValue b a

  /// <summary>An alternative to the aggregate/fold/reduce function with an option to return early
  /// (i.e.: before the whole sequence is read).</summary>
  /// <usage>
  /// [| 1; 2; 3; 4; 5; 6; 7 |]
  /// |> aggregateIf (fun acc item continueWith ->
  ///                  if (item > 5)
  ///                  then acc
  ///                  elif (item % 2 = 0)
  ///                  then continueWith (acc)
  ///                  else continueWith (item :: acc)) []
  /// This gives a list of odd items smaller or equal to 5: [| 1; 3; 5 |]
  /// </usage>
  /// <param name="func">The function which specifies how each item should be compared and combined
  /// with the accumulator.</param>
  /// <param name="acc">The accumulator which stores the result of the function</param>
  /// <param name="col>The inptu collection</param>
  /// <returns>The resulting accumulator.</returns>
  let rec aggregateIf func (acc: 'State) col =
    match col with
    | [] -> acc
    | head :: tail -> func acc head (fun subAccumulator -> aggregateIf func subAccumulator tail)

  /// <summary>Converts a value from discriminated union into string.</summary>
  /// <param name="x">Value from a discriminated union.</param>
  /// <returns>The value in string format.</returns>
  let unionToString (x: 'a) =
    match FSharpValue.GetUnionFields(x, typeof<'a>) with
    | case, _ -> case.Name

  /// <summary>Converts a string into a value from discriminated union.</summary>
  /// <param name="s">The input string.</param>
  /// <returns>A value from given discriminated union.</returns>
  let unionFromString<'a> (s: string) =
    match FSharpType.GetUnionCases typeof<'a> |> Array.filter (fun case -> case.Name = s) with
    | [| case |] -> Some(FSharpValue.MakeUnion(case, [||]) :?> 'a)
    | _ -> None

  /// <summary>Builds a PTV source ID from given int.</summary>
  /// <param name="id">An external integer ID.</param>
  /// <returns>A string in the format "pharma-"{id}</returns>
  let buildSourceId (id: int) =
    Defaults.sourceIdPrefix + id.ToString()
      
  /// <summary>An active pattern for matching regexes.</summary>
  /// <param name="pattern">The regex pattern that should be evaluated.</param>
  /// <param name="input>Tested string.</param>
  /// <returns>An Option<string>, with Some value, if the pattern is found. Otherwise, None is returned.</returns>
  let (|Regex|_|) pattern input =
    let m = Regex.Match(input, pattern)
    if m.Success
    then Some m.Value
    else None
    
  /// <summary>An active pattern for matching empty strings.</summary>
  /// <param name="text">Tested string.</param>
  /// <returns>None, if the string is empty. Otherwise, returns some with the string value.</returns>
  let (|EmptyString|_|) text =
    if String.IsNullOrWhiteSpace text
    then None
    else Some text
  
  /// <summary>Splits a sequence into smaller batches of the same size. Last sequence may be smaller.</summary>
  /// <param name="xs">Input sequence.</param>
  /// <returns>A list of lists.</returns>
  let batch length (xs: seq<'T>) =
      let rec loop xs =
          [
              yield Seq.truncate length xs |> Seq.toList
              match Seq.length xs <= length with
              | false -> yield! loop (Seq.skip length xs)
              | true -> ()
          ]
      loop xs
