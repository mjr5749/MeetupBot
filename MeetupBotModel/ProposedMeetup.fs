﻿module MeetupBot.Meetup.ProposedMeetup

open System

let proposeMeetup' description location (targetDates: DateRange) =
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

let proposeMeetup'' description location (targetDates: DateRange) =
    match targetDates with
    | (first, last) when first.Date < DateTime.Now.Date -> 
        failwith "Invalid first date"
    | (first, last) when first.Date > last.Date -> 
        failwith "Invalid last date"
    | (first, last) ->
        Some { 
            Description=description; 
            Location=location;
            TargetDates=(first.Date, last.Date); 
            Participants=[]
        }

let private makeValidTargetDates (first:DateTime, last:DateTime) =
    let today = DateTime.Now.Date
    let oneYearFromToday = today.AddDays(365.0).Date

    let first = max today first
    let first = min first oneYearFromToday
  
    let last = max first last 
    let last = min last (first.AddDays(365.0))

    (first.Date, last.Date)


let proposeMeetup description location (targetDates: DateRange) =
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

let isDateInRange (date:DateTime) meetup =
    let (first,last) = meetup.TargetDates 
    first <= date.Date && date.Date <= last
       
let private removeDate participant (date:DateTime) =
    let removeDate = (fun (d:DateTime) -> d.Date <> date.Date)
    { participant with
        AvailableDates = List.filter removeDate participant.AvailableDates;
        UnavailableDates = List.filter removeDate participant.UnavailableDates;
    }

let addAvailableDate username date meetup =
    match isDateInRange date meetup, getParticipant username meetup with
    | true, Some participant ->
        let participant = removeDate participant date
        updateParticipant { participant with 
                              AvailableDates=(date.Date :: participant.AvailableDates) 
                          } meetup
    | _ -> meetup

let addUnavailableDate username date meetup =
    match isDateInRange date meetup, getParticipant username meetup with
    | true, Some participant ->
        let participant = removeDate participant date
        updateParticipant { participant with 
                              UnavailableDates=(date.Date :: participant.UnavailableDates) 
                          } meetup
    | _ -> meetup

let availabilityCheck' meetup username (date:DateTime) =
    let dateSearch = (fun (d:DateTime) -> d.Date = date.Date)
    match getParticipant username meetup with
    | Some participant -> 
        match List.tryFind dateSearch participant.AvailableDates with
        | Some _ -> Some Available
        | None -> 
            match List.tryFind dateSearch participant.UnavailableDates with
            | Some _ -> Some Unavailable
            | None -> Some Unspecified
    | None -> None
    
let availabilityCheck'' meetup username (date:DateTime) =
    match getParticipant username meetup with
    | Some participant ->
        printfn "Optimizing date search..." 
        let availableDates = participant.AvailableDates |> Set.ofList
        let unavailableDates = participant.UnavailableDates |> Set.ofList

        if Set.contains date availableDates then Some Available
        else if Set.contains date unavailableDates then Some Unavailable
        else Some Unspecified
    | None -> None

let availabilityCheck''' meetup username =
    match getParticipant username meetup with
    | Some participant ->
        printfn "Optimizing date search..." 
        let availableDates = participant.AvailableDates |> Set.ofList
        let unavailableDates = participant.UnavailableDates |> Set.ofList

        fun (date:DateTime) ->
            if Set.contains date availableDates then Some Available
            else if Set.contains date unavailableDates then Some Unavailable
            else Some Unspecified
    | None -> (fun _ -> None)

let availabilityCheck meetup username =
    match getParticipant username meetup with
    | Some participant ->
        let availableDates = lazy ( 
            printfn "Optimizing available date search..."
            participant.AvailableDates |> Set.ofList )
        let unavailableDates = lazy ( 
            printfn "Optimizing unavailable date search..."
            participant.UnavailableDates |> Set.ofList )

        fun (date:DateTime) ->
            if Set.contains date (availableDates.Force()) then Some Available
            else if Set.contains date (unavailableDates.Force()) then Some Unavailable
            else Some Unspecified
    | None -> (fun _ -> None)
    