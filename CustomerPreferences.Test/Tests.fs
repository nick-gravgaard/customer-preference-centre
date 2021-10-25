module Tests

open System
open Xunit

open CustomerPreferences.DomainTypes
open CustomerPreferences.CommonLibrary


[<Fact>]
let ``test stringToFrequency`` () =
    Assert.Equal((stringToFrequency "Every day"), Some EveryDay)
    Assert.Equal((stringToFrequency "Never"), Some Never)
    Assert.Equal((stringToFrequency "On the 10th of the month"), Some <| OnDateOfMonth 10)
    Assert.Equal((stringToFrequency "On the 0th of the month"), None)
    Assert.Equal((stringToFrequency "On the 29th of the month"), None)
    Assert.Equal((stringToFrequency "On Tuesday and Friday"), Some <| OnDaysOfWeek [DayOfWeek.Tuesday; DayOfWeek.Friday])
    Assert.Equal((stringToFrequency "On XYZ and Friday"), Some <| OnDaysOfWeek [DayOfWeek.Friday])
    Assert.Equal((stringToFrequency "something else"), None)


[<Fact>]
let ``test produceReport`` () =
    let frequenciesByCustomer =
        Map [ ("A", "Every day")
              ("B", "On the 10th of the month")
              ("C", "On Tuesday and Friday")
            ]
    let report = produceReport (DateTime(2018, 04, 01)) 14 frequenciesByCustomer
    let output = reportOutput report
    let expected =
        [ "Sun 01-April-2018  A"
          "Mon 02-April-2018  A"
          "Tue 03-April-2018  A, C"
          "Wed 04-April-2018  A"
          "Thu 05-April-2018  A"
          "Fri 06-April-2018  A, C"
          "Sat 07-April-2018  A"
          "Sun 08-April-2018  A"
          "Mon 09-April-2018  A"
          "Tue 10-April-2018  A, B, C"
          "Wed 11-April-2018  A"
          "Thu 12-April-2018  A"
          "Fri 13-April-2018  A, C"
          "Sat 14-April-2018  A"
        ] |> String.concat Environment.NewLine
    Assert.Equal(output, expected)
