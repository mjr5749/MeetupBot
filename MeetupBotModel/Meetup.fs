namespace MeetupBot.Meetup 

open System

type DateRange = DateTime * DateTime
type DateList = Set<DateTime>

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

type Availability =
    | Available
    | Unavailable
    | Unspecified