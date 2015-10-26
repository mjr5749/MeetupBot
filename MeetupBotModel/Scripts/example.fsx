#I __SOURCE_DIRECTORY__
#load "load-references.fsx"
#load "../AssemblyInfo.fs"
      "../Meetup.fs"
      "../ProposedMeetup.fs"

open System;
open MeetupBot.Meetup;
open MeetupBot.Meetup.ProposedMeetup;

let meetup = proposeMeetup "F# Meetup" "Rochester, NY" 
                           (DateTime.Now, DateTime.Now.AddDays(30.0))
             |> addParticipant (Username "mrichards")
             |> changeParticipantRole (Username "mrichards") Presenter

