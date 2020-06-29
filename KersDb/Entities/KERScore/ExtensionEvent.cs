namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// The type Event.
    /// </summary>
    public partial class ExtensionEvent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    
        /// <summary>
        /// Gets or sets has attachments.
        /// Set to true if the event has attachments.
        /// </summary>
        public bool? HasAttachments { get; set; }
    
        /// <summary>
        /// Gets or sets subject.
        /// The text of the event's subject line.
        /// </summary>
        public string Subject { get; set; }
    
        /// <summary>
        /// Gets or sets body.
        /// The body of the message associated with the event. It can be in HTML or text format.
        /// </summary>
        public string Body { get; set; }
    
        /// <summary>
        /// Gets or sets body preview.
        /// The preview of the message associated with the event. It is in text format.
        /// </summary>
        public string BodyPreview { get; set; }
    
        /// <summary>
        /// Gets or sets start.
        /// The date, time, and time zone that the event starts.
        /// </summary>
        public DateTimeOffset Start { get; set; }
        public bool HasStartTime { get; set; }
        //public ExtensionEventDateTimeTimeZone Start { get; set; }
    
        /// <summary>
        /// Gets or sets end.
        /// The date, time, and time zone that the event ends.
        /// </summary>
        public DateTimeOffset? End { get; set; }
        public bool HasEndTime { get; set; }
        //public ExtensionEventDateTimeTimeZone End { get; set; }
    
        /// <summary>
        /// Gets or sets location.
        /// The location of the event.
        /// </summary>
        public ExtensionEventLocation Location { get; set; }
    
        /// <summary>
        /// Gets or sets locations.
        /// The locations where the event is held or attended from. The location and locations properties always correspond with each other. If you update the location property, any prior locations in the locations collection would be removed and replaced by the new location value.
        /// </summary>
        public IEnumerable<ExtensionEventLocation> Locations { get; set; }
    
        /// <summary>
        /// Gets or sets is all day.
        /// Set to true if the event lasts all day.
        /// </summary>
        public bool? IsAllDay { get; set; }
    
        /// <summary>
        /// Gets or sets is cancelled.
        /// Set to true if the event has been canceled.
        /// </summary>
        public bool? IsCancelled { get; set; }
    
        /// <summary>
        /// Gets or sets recurrence.
        /// The recurrence pattern for the event.
        /// </summary>
        public ExtensionEventPatternedRecurrence Recurrence { get; set; }
    
        /// <summary>
        /// Gets or sets series master id.
        /// The ID for the recurring series master item, if this event is part of a recurring series.
        /// </summary>
        public string SeriesMasterId { get; set; }
    
    
        /// <summary>
        /// Gets or sets type.
        /// The event type. The possible values are: singleInstance, occurrence, exception, seriesMaster. Read-only.
        /// </summary>
        public ExtensionEventType? Type { get; set; }
    
        /// <summary>
        /// Gets or sets attendees.
        /// The collection of attendees for the event.
        /// </summary>
        public ICollection<KersUser> Attendees { get; set; }
    
        /// <summary>
        /// Gets or sets organizer.
        /// The organizer of the event.
        /// </summary>
        public KersUser Organizer { get; set; }

        public int OrganizerId { get; set; }
    
        /// <summary>
        /// Gets or sets web link.
        /// The URL to open the event in Outlook Web App.The event will open in the browser if you are logged in to your mailbox via Outlook Web App. You will be prompted to login if you are not already logged in with the browser.This URL can be accessed from within an iFrame.
        /// </summary>
        public string WebLink { get; set; }
    
        /// <summary>
        /// Gets or sets online meeting url.
        /// A URL for an online meeting. The property is set only when an organizer specifies an event as an online meeting such as a Skype meeting. Read-only.
        /// </summary>
        public string OnlineMeetingUrl { get; set; }

        /// <summary>
        /// Gets or sets Images Associated with the Event.
        /// A list of images that are uploaded to the Event
        /// </summary>
        public ICollection<ExtensionEventImage> ExtensionEventImages {get;set;}

        /// <summary>
        /// Gets or sets created date time.
        /// The Timestamp type represents date and time information using ISO 8601 format and is always in UTC time. For example, midnight UTC on Jan 1, 2014 would look like this: '2014-01-01T00:00:00Z'
        /// </summary>
        public DateTimeOffset? CreatedDateTime { get; set; }
    
        /// <summary>
        /// Gets or sets last modified date time.
        /// The Timestamp type represents date and time information using ISO 8601 format and is always in UTC time. For example, midnight UTC on Jan 1, 2014 would look like this: '2014-01-01T00:00:00Z'
        /// </summary>
        public DateTimeOffset? LastModifiedDateTime { get; set; }
        
        [StringLength(200)]
        public string tContact { get; set; }

        [StringLength(300)]
        public string tLocation { get; set; }

        public string DiscriminatorValue {
            get {
                return this.GetType().Name;
            }
        }

    }
}

