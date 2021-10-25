namespace CustomerPreferences

open System

// Stop linter complaining about record field names starting with lower case characters
// (which seems a common style - see for instance code from fsharpforfunandprofit.com)
// fsharplint:disable RecordFieldNames

module DomainTypes =
    type Frequency =
        | OnDateOfMonth of int
        | OnDaysOfWeek of list<DayOfWeek>
        | EveryDay
        | Never


    type DaysContacts =
        { date: DateTime
          customerNames: list<string>
        }


module CommonLibrary =

    open DomainTypes

    let tryParseWith (tryParseFunc: string -> bool * _) = tryParseFunc >> function
        | true, v -> Some v
        | false, _ -> None


    let parseDateTime = tryParseWith DateTime.TryParse
    let parseDayOfWeek = tryParseWith DayOfWeek.TryParse
    let parseInt = tryParseWith Int32.TryParse


    let stringToFrequency frequencyString: option<Frequency> =
        let (|Prefix|_|) (p:string) (s:string) =
            if s.StartsWith(p) then
                Some(s.Substring(p.Length))
            else
                None

        match frequencyString with
        | "Every day" -> Some EveryDay
        | "Never" -> Some Never
        | Prefix "On the " remainingText ->
            let numberString =
                remainingText
                |> Seq.toList
                |> Seq.takeWhile Char.IsDigit
                |> Seq.map string
                |> String.concat ""
            let didParse, num = Int32.TryParse numberString
            if didParse && num >= 1 && num <= 28 then
                Some <| OnDateOfMonth num
            else
                None
        | Prefix "On " remainingText ->
            let dayOfWeekStrings =
                remainingText
                    .Replace(" and ", " ")
                    .Split(' ')
            let daysOfWeek =
                dayOfWeekStrings
                |> Seq.toList
                |> List.map parseDayOfWeek
                |> List.choose id
            Some <| OnDaysOfWeek daysOfWeek
        | _ -> None


    let customersForDate (date: DateTime) (frequenciesByCustomer: Map<string, string>) =
        frequenciesByCustomer
            |> Map.toList
            |> List.map
                ( fun (customerName, frequencyString) ->
                    match stringToFrequency frequencyString with
                    | None -> None
                    | Some frequency ->
                        match frequency with
                        | EveryDay -> Some customerName
                        | Never -> None
                        | OnDateOfMonth (dayOfMonth: int ) ->
                            if dayOfMonth = date.Day then
                                Some customerName
                            else
                                None
                        | OnDaysOfWeek daysOfWeek ->
                            if List.contains date.DayOfWeek daysOfWeek then
                                Some customerName
                            else
                                None
                )
            |> List.choose id


    let parseInput (input: list<string>): Map<string, string> =
        let splitLines = input |> List.map (fun line -> line.Split ";")

        let tuples =
            splitLines
            |> List.map
                (fun splitLine ->
                    if splitLine.Length >= 2 then
                        Some (splitLine.[0], splitLine.[1])
                    else
                        None
                )
            |> List.choose id

        Map.ofList tuples


    let produceReport (startDate: DateTime) (nofDays: int) (frequenciesByCustomer: Map<string, string>) =
        seq { 0 .. nofDays - 1 }
        |> Seq.toList
        |> List.map
            ( fun dayNum ->
                let date = startDate.AddDays(float(dayNum))
                let customerNames = customersForDate date frequenciesByCustomer
                { date = date
                  customerNames = customerNames
                }
            )


    let reportOutput (report: list<DaysContacts>) =
        report
        |> List.map
            (fun daysContacts ->
                let dateString = daysContacts.date.ToString "ddd dd-MMMM-yyyy"
                let customers = daysContacts.customerNames |> String.concat ", "
                sprintf "%s  %s" dateString customers
            )
        |> String.concat Environment.NewLine
