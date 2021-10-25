open System

open CustomerPreferences.DomainTypes
open CustomerPreferences.CommonLibrary


[<EntryPoint>]
let main argv =
    let exampleUsage  = "cat example-input.txt | dotnet run Program.fs 2018-04-01 90"
    if argv.Length = 3 then
        match (parseDateTime argv.[1], parseInt argv.[2]) with
        | (Some startDate, Some nofDays) ->

            let stdinLines =
                Seq.initInfinite
                    (fun _ -> Console.ReadLine())
                |> Seq.takeWhile (fun line -> line <> null)
                |> Seq.toList

            let frequenciesByCustomer = parseInput stdinLines
            let report = produceReport startDate nofDays frequenciesByCustomer
            printfn "%s" <| reportOutput report

            0
        | _ ->
            printfn "Error: Start date and/or duration arguments were malformed. Example usage:"
            printfn "%s" exampleUsage
            1
    else
        printfn "Error: Expected start date and duration arguments. Example usage:"
        printfn "%s" exampleUsage
        1
