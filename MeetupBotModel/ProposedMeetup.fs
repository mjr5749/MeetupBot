module MeetupBot.Meetup.ProposedMeetup

open System

let private stripTime (date:DateTime) = date.Date

let private isDateInRange (date:DateTime) (targetDates:DateRange) =
    let (first,last) = targetDates 
    first <= date.Date && date.Date <= last

let private toValidMeetupDates dates targetDates =
    dates
    |> Set.map stripTime
    |> Set.filter (fun date -> isDateInRange date targetDates)

let private makeValidTargetDates (first:DateTime, last:DateTime) =
    let today = DateTime.Now.Date
    let oneYearFromToday = today.AddDays(365.0).Date

    let first = max today first
    let first = min first oneYearFromToday
  
    let last = max first last 
    let last = min last (first.AddDays(365.0))

    (first.Date, last.Date)


let proposeMeetup description location (targetDates:DateRange) =
    { 
        Description=description; 
        TargetDates=makeValidTargetDates targetDates; 
        Location=location; 
        Participants=[]
    }

let changeDescription description meetup =
    { meetup with Description = description }

let changeLocation location meetup =
    { meetup with Location = location }

let changeTargetDates (targetDates:DateRange) meetup =
    let targetDates = makeValidTargetDates targetDates
    { meetup with
        TargetDates = targetDates
        Participants = 
            meetup.Participants
            |> List.map (fun p ->
                { p with
                    AvailableDates = toValidMeetupDates p.AvailableDates targetDates
                    UnavailableDates = toValidMeetupDates p.UnavailableDates targetDates
                }
            )
    }

let private createParticipant username =
    {
        Username = username;
        Role = Attendee;
        AvailableDates = Set.empty;
        UnavailableDates = Set.empty;
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
    let removeUser = List.filter (fun p -> p.Username <> username)
    { meetup with
        Participants = removeUser meetup.Participants
    }

let private updateParticipant participant meetup =
    let meetup = removeParticipant participant.Username meetup 
    { meetup with Participants = participant :: meetup.Participants }

let changeParticipantRole username (role:ParticipantRole) meetup =
    match getParticipant username meetup with
    | Some participant -> 
        updateParticipant { participant with Role=role } meetup
    | None -> meetup

let resetAllDates username meetup =
    match getParticipant username meetup with
    | Some participant ->
        updateParticipant { participant with 
                              AvailableDates = Set.empty 
                              UnavailableDates = Set.empty 
                          } meetup
    | None -> meetup
          
let resetDates username dates meetup =
    let dates = toValidMeetupDates dates meetup.TargetDates
    match getParticipant username meetup with
    | Some participant ->
        updateParticipant { participant with 
                              AvailableDates = Set.difference participant.AvailableDates dates
                              UnavailableDates = Set.difference participant.UnavailableDates dates
                          } meetup
    | None -> meetup

let addAvailableDates username dates meetup =
    let dates = toValidMeetupDates dates meetup.TargetDates
    match getParticipant username meetup with
    | Some participant ->
        updateParticipant { participant with 
                              AvailableDates = Set.union participant.AvailableDates dates
                              UnavailableDates = Set.difference participant.UnavailableDates dates
                          } meetup
    | None -> meetup

let addAvailableDate username date meetup =
    addAvailableDates username (Set.singleton date) meetup

let addUnavailableDates username dates meetup =
    let dates = toValidMeetupDates dates meetup.TargetDates
    match getParticipant username meetup with
    | Some participant ->
        updateParticipant { participant with 
                              AvailableDates = Set.difference participant.AvailableDates dates
                              UnavailableDates = Set.union participant.UnavailableDates dates
                          } meetup
    | None -> meetup

let addUnavailableDate username date meetup =
    addUnavailableDates username (Set.singleton date) meetup

let checkAvailability meetup username (date:DateTime) =
    if isDateInRange date meetup.TargetDates then
        match getParticipant username meetup with
        | Some participant ->
            if Set.contains date.Date participant.AvailableDates then Some Available
            else if Set.contains date.Date participant.UnavailableDates then Some Unavailable
            else Some Unspecified
        | None -> None
    else None
    