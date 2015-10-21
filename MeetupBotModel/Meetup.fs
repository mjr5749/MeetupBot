module MeetupBot.Meetup 

open System

type DateRange = DateTime * DateTime
type DateList = DateTime list

type Username = Username of string
type ParticipantRole = 
    | Host
    | Presenter
    | Attendee

type ParticipantSchedulingInfo = {
    Username: Username;
    Role: ParticipantRole;
    AvailableDates: DateList;
    UnavailableDates: DateList;
}

type ProposedMeetupInfo = {
    Description: string;
    Location: string;
    TargetDates: DateRange;
    Participants: ParticipantSchedulingInfo list;
}

type Meetup = 
    | ProposedMeetup of ProposedMeetupInfo
    | ScheduledMeetup

let proposeMeetup1 description location (targetDates: DateRange) =
    match targetDates with
    | (first, last) when first.Date < DateTime.Now.Date -> None
    | (first, last) when first.Date > last.Date -> None
    | (first, last) ->
        Some { 
            Description=description; 
            Location=location;
            TargetDates=(first.Date, last.Date); 
            Participants=[]
        }

let makeValidTargetDates (first:DateTime, last:DateTime) =
    let today = DateTime.Now.Date
    let oneYearFromToday = today.AddDays(365.0).Date

    let first = max today first
    let first = min first oneYearFromToday
  
    let last = max first last 
    let last = min last (first.AddDays(365.0))

    (first.Date, last.Date)


let proposeMeetup2 description location (targetDates: DateRange) =
    { 
        Description=description; 
        TargetDates=makeValidTargetDates targetDates; 
        Location=location; 
        Participants=[]
    }

let private createParticipant username =
    {
        Username = username;
        Role = Attendee;
        AvailableDates = [];
        UnavailableDates = [];
    }

let private removeParticipantInternal
    username (participants: ParticipantSchedulingInfo list) =
    participants
    |> List.filter (fun p -> p.Username <> username)

let private updateParticipant participant meetup =
    { meetup with
        Participants = participant ::
                       (removeParticipantInternal participant.Username meetup.Participants) 
    }

let getParticipant username meetup =
    meetup.Participants
    |> List.tryFind (fun p -> p.Username = username)

let addParticipant username meetup = 
    match getParticipant username meetup with
    | Some participant -> meetup
    | None -> { meetup with
                    Participants = (createParticipant username) :: meetup.Participants
              }

let removeParticipant username meetup =
    { meetup with
        Participants = removeParticipantInternal username meetup.Participants
    }