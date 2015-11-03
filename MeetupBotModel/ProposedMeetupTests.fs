module MeetupBot.Meetup.ProposedMeetupTests

open NUnit.Framework
open FsUnit
open FsUnit.TopLevelOperators

open ProposedMeetup

[<Test>] [<Category("Some category")>]
let ``some test`` () = 10 |> should equal 10

[<TestFixture>]
type AccountTest() =
  [<Test>]
  member x.SimpleTest() = 
    1 |> should equal 1