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
        |> addAvailableDate x.mrichards (DateTime.Now.AddDays(1.0).Date)

    [<Test>]
    member x.``meetup has one participant that is the presenter`` () = 
        let meetup = x.CreateBasicMeetup()
        meetup.Participants.Length |> should equal 1
        
        let participant = 
            meetup
            |> getParticipant x.mrichards |> Option.get

        participant.Role |> should equal Presenter
        participant.AvailableDates.Length |> should equal 1

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

        participant.UnavailableDates.Length |> should equal 1

    [<Test>]
    member x.``adding a date outside of the target date range does nothing`` () =
        let date = DateTime.Now.AddDays(-3.0).Date
        let participant = 
            x.CreateBasicMeetup()
            |> addAvailableDate x.mrichards date
            |> addUnavailableDate x.mrichards date
            |> getParticipant x.mrichards
            |> Option.get

        participant.AvailableDates.Length |> should equal 1
        participant.UnavailableDates.Length |> should equal 0
                      
    [<Test>]
    member x.``participant default role is Attendee`` () =
        let user = (Username "testuser")
        let participant =
            x.CreateBasicMeetup()
            |> addParticipant user
            |> getParticipant user
            |> Option.get

        participant.Role |> should equal Attendee

    [<Test>]
    member x.``participant can change role`` () =
        let participant =
            x.CreateBasicMeetup()
            |> changeParticipantRole x.mrichards Attendee
            |> getParticipant x.mrichards |> Option.get

        participant.Role |> should equal Attendee

    [<Test>]
    member x.``all participants can be removed`` () =
        let meetup =
            x.CreateBasicMeetup()
            |> removeParticipant x.mrichards
            
        meetup.Participants |> should equal []