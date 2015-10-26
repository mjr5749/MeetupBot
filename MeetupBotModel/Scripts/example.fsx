#I __SOURCE_DIRECTORY__
#load "load-references.fsx"
#load "../AssemblyInfo.fs"
      "../Meetup.fs"
      "../ProposedMeetup.fs"

open System;
open MeetupBot.Meetup;
open MeetupBot.Meetup.ProposedMeetup;

// Create a proposed meetup event
let meetup = proposeMeetup "F# Meetup" "Rochester, NY" 
                           (DateTime.Now, DateTime.Now.AddDays(30.0))
             |> addParticipant (Username "mrichards")
             |> changeParticipantRole (Username "mrichards") Presenter
             |> addAvailableDate (Username "mrichards") (DateTime.Parse("2015-11-07"))

// Adding a date as unavailable removes it from the available dates
let meetup2 = 
    meetup
    |> addAvailableDate (Username "mrichards") (DateTime.Parse("2015-11-08"))
    |> addUnavailableDate (Username "mrichards") (DateTime.Parse("2015-11-08"))

// Adding a date outside of the target dates for the meetup - does nothing
let meetup3 =
    meetup2
    |> addAvailableDate (Username "mrichards") (DateTime.Now.AddDays(-7.0))
                     