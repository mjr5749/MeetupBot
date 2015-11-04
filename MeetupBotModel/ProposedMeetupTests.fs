module MeetupBot.Meetup.ProposedMeetupTests

open System
open NUnit.Framework
open FsUnit
open FsUnit.TopLevelOperators

open ProposedMeetup

[<TestFixture>]
[<Category("Basic ProposedMeetup Tests")>]
type BasicProposedMeetupTests() =
    member x.mrichards = Username "mrichards"
    member x.CreateBasicMeetup() = 
        proposeMeetup "F# Meetup" "Rochester, NY" (DateTime.Now, DateTime.Now.AddDays(30.0))
        |> addParticipant x.mrichards
        |> changeParticipantRole x.mrichards Presenter
        |> addAvailableDate x.mrichards (DateTime.Parse("2015-11-04"))

    [<Test>]
    member x.``meetup has one participant`` () = 
        x.CreateBasicMeetup().Participants.Length |> should equal 1

    [<Test>]
    member x.``participant has one available date`` () =
        x.CreateBasicMeetup().Participants.Head.AvailableDates.Length 
        |> should equal 1

    [<Test>]
    member x.``adding an unavailable date removes it from available dates`` () =
        let date = DateTime.Now.AddDays(3.0).Date
        let participant = 
            x.CreateBasicMeetup()
            |> addAvailableDate x.mrichards date
            |> addUnavailableDate x.mrichards date
            |> getParticipant x.mrichards
            |> Option.get

        List.tryFind (fun d -> d = date) participant.AvailableDates
        |> should equal None
        
        List.tryFind (fun d -> d = date) participant.UnavailableDates
        |> should equal (Some date)

    [<Test>]
    member x.``adding an unavailable date twice does not duplicate it`` () =
        let date = DateTime.Now.AddDays(3.0).Date
        let participant = 
            x.CreateBasicMeetup()
            |> addUnavailableDate x.mrichards date
            |> addUnavailableDate x.mrichards date
            |> getParticipant x.mrichards
            |> Option.get

        participant.UnavailableDates.Length
        |> should equal 1

    [<Test>]
    member x.``adding a date outside of the target date range does nothing`` () =
        let date = DateTime.Now.AddDays(-3.0).Date
        let participant = 
            x.CreateBasicMeetup()
            |> addAvailableDate x.mrichards date
            |> addUnavailableDate x.mrichards date
            |> getParticipant x.mrichards
            |> Option.get

        participant.AvailableDates.Length
        |> should equal 1

        participant.UnavailableDates.Length
        |> should equal 0
                      