module MeetupBot.Meetup 

open System

type DateRange = DateTime * DateTime
type DateList = DateTime list

type Username = Username of string
type ParticipantRole = 
    | Host
    | Presenter
    | Attendee

type Location = Location of string
type LocationList = Location list

type ParticipantSchedulingInfo = {
    Username: Username;
    Role: ParticipantRole;
    AvailableDates: DateList;
    UnavailableDates: DateList;
}

type ProposedMeetupInfo = {
    Description: string;
    TargetDates: DateRange;
    PotentialLocations: LocationList;
    Participants: ParticipantSchedulingInfo list;
}

type Meetup = 
    | ProposedMeetup of ProposedMeetupInfo
    | ScheduledMeetup

let proposeMeetup1 description (targetDates: DateRange) =
    match targetDates with
    | (first, last) when first.Date < DateTime.Now.Date -> None
    | (first, last) when first.Date > last.Date -> None
    | (first, last) ->
        Some { 
            Description=description; 
            TargetDates=(first.Date, last.Date); 
            PotentialLocations=[]; 
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


let proposeMeetup2 description (targetDates: DateRange) =
    { 
        Description=description; 
        TargetDates=makeValidTargetDates targetDates; 
        PotentialLocations=[]; 
        Participants=[]
    }