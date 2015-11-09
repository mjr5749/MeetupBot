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
        participant.AvailableDates.Count |> should equal 1

    [<Test>]
    member x.``participant has one available date`` () =
        x.CreateBasicMeetup().Participants.Head.AvailableDates.Count
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

        Set.contains date participant.AvailableDates
        |> should equal false
        
        Set.contains date participant.UnavailableDates
        |> should equal true 

    [<Test>]
    member x.``adding an unavailable date twice does not duplicate it`` () =
        let date = DateTime.Now.AddDays(3.0).Date
        let participant = 
            x.CreateBasicMeetup()
            |> addUnavailableDate x.mrichards date
            |> addUnavailableDate x.mrichards date
            |> getParticipant x.mrichards
            |> Option.get

        participant.UnavailableDates.Count |> should equal 1

    [<Test>]
    member x.``adding a date outside of the target date range does nothing`` () =
        let date = DateTime.Now.AddDays(-3.0).Date
        let participant = 
            x.CreateBasicMeetup()
            |> addAvailableDate x.mrichards date
            |> addUnavailableDate x.mrichards date
            |> getParticipant x.mrichards
            |> Option.get

        participant.AvailableDates.Count |> should equal 1
        participant.UnavailableDates.Count |> should equal 0
                      
    [<Test>] 
    member x.``adding multiple available dates at once`` () =
        let dates = 
            [0 .. 5] // 6 weeks
            |> List.map ( (*) 7 >> float >> DateTime.Now.AddDays )

        let meetup =
            x.CreateBasicMeetup()
            |> resetAllDates x.mrichards
            |> addAvailableDates x.mrichards (Set dates)
        
        let checkUserAvail = checkAvailability meetup x.mrichards

        dates 
        |> List.map checkUserAvail
        |> should equal 
            [ Some(Available); Some(Available); Some(Available);
              Some(Available); Some(Available); 
              None (* outside target dates *) ]

    [<Test>] 
    member x.``changing target dates is reflected in checkAvailability`` () =
        let dates = 
            [0 .. 5] // 6 weeks
            |> List.map ( (*) 7 >> float >> DateTime.Now.AddDays )

        let meetup =
            x.CreateBasicMeetup()
            |> resetAllDates x.mrichards
            |> addAvailableDates x.mrichards (Set dates)
            |> changeTargetDates (DateTime.Now.AddDays(2.0), DateTime.Now.AddDays(60.0))
        
        let checkUserAvail = checkAvailability meetup x.mrichards

        dates 
        |> List.map checkUserAvail
        |> should equal 
            [ None; (* outside target dates *)
              Some(Available); Some(Available);
              Some(Available); Some(Available); 
              Some(Unspecified) (* was not valid when addAvailableDates called *) ]

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

[<TestFixture>]
[<Category("Availability Check Tests")>]
type AvailabiltyCheckTests() =
    member x.mrichards = Username "mrichards"
    member x.CreateMeetup() = 
        proposeMeetup "Availability Checks" "Rochester, NY" (DateTime.Now, DateTime.Now.AddDays(30.0))
        |> addParticipant x.mrichards
        |> addAvailableDate x.mrichards (DateTime.Now.AddDays(1.0).Date)
        |> addAvailableDate x.mrichards (DateTime.Now.AddDays(3.0).Date)
        |> addUnavailableDate x.mrichards (DateTime.Now.AddDays(2.0).Date)
        |> addUnavailableDate x.mrichards (DateTime.Now.AddDays(4.0).Date)

    [<Test>]
    member x.``availability check works for Available,Unavailable and Unspecified`` () =
        let meetup = x.CreateMeetup()
        let checkUserAvail = checkAvailability meetup x.mrichards

        [1 .. 7] 
        |> List.map (float >> DateTime.Now.AddDays >> checkUserAvail)
        |> should equal 
            [Some(Available); Some(Unavailable);
             Some(Available); Some(Unavailable);
             Some(Unspecified); Some(Unspecified); Some(Unspecified) ]
