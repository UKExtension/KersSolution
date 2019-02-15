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
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ExtensionEvent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    
        /// <summary>
        /// Gets or sets has attachments.
        /// Set to true if the event has attachments.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "hasAttachments", Required = Newtonsoft.Json.Required.Default)]
        public bool? HasAttachments { get; set; }
    
        /// <summary>
        /// Gets or sets subject.
        /// The text of the event's subject line.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "subject", Required = Newtonsoft.Json.Required.Default)]
        public string Subject { get; set; }
    
        /// <summary>
        /// Gets or sets body.
        /// The body of the message associated with the event. It can be in HTML or text format.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "body", Required = Newtonsoft.Json.Required.Default)]
        public string Body { get; set; }
    
        /// <summary>
        /// Gets or sets body preview.
        /// The preview of the message associated with the event. It is in text format.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "bodyPreview", Required = Newtonsoft.Json.Required.Default)]
        public string BodyPreview { get; set; }
    
        /// <summary>
        /// Gets or sets start.
        /// The date, time, and time zone that the event starts.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "start", Required = Newtonsoft.Json.Required.Default)]
        public DateTimeOffset Start { get; set; }
        //public ExtensionEventDateTimeTimeZone Start { get; set; }
    
        /// <summary>
        /// Gets or sets end.
        /// The date, time, and time zone that the event ends.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "end", Required = Newtonsoft.Json.Required.Default)]
        public DateTimeOffset End { get; set; }
        //public ExtensionEventDateTimeTimeZone End { get; set; }
    
        /// <summary>
        /// Gets or sets location.
        /// The location of the event.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "location", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventLocation Location { get; set; }
    
        /// <summary>
        /// Gets or sets locations.
        /// The locations where the event is held or attended from. The location and locations properties always correspond with each other. If you update the location property, any prior locations in the locations collection would be removed and replaced by the new location value.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "locations", Required = Newtonsoft.Json.Required.Default)]
        public IEnumerable<ExtensionEventLocation> Locations { get; set; }
    
        /// <summary>
        /// Gets or sets is all day.
        /// Set to true if the event lasts all day.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "isAllDay", Required = Newtonsoft.Json.Required.Default)]
        public bool? IsAllDay { get; set; }
    
        /// <summary>
        /// Gets or sets is cancelled.
        /// Set to true if the event has been canceled.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "isCancelled", Required = Newtonsoft.Json.Required.Default)]
        public bool? IsCancelled { get; set; }
    
        /// <summary>
        /// Gets or sets recurrence.
        /// The recurrence pattern for the event.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "recurrence", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventPatternedRecurrence Recurrence { get; set; }
    
        /// <summary>
        /// Gets or sets series master id.
        /// The ID for the recurring series master item, if this event is part of a recurring series.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "seriesMasterId", Required = Newtonsoft.Json.Required.Default)]
        public string SeriesMasterId { get; set; }
    
    
        /// <summary>
        /// Gets or sets type.
        /// The event type. The possible values are: singleInstance, occurrence, exception, seriesMaster. Read-only.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventType? Type { get; set; }
    
        /// <summary>
        /// Gets or sets attendees.
        /// The collection of attendees for the event.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "attendees", Required = Newtonsoft.Json.Required.Default)]
        public ICollection<KersUser> Attendees { get; set; }
    
        /// <summary>
        /// Gets or sets organizer.
        /// The organizer of the event.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "organizer", Required = Newtonsoft.Json.Required.Default)]
        public KersUser Organizer { get; set; }
    
        /// <summary>
        /// Gets or sets web link.
        /// The URL to open the event in Outlook Web App.The event will open in the browser if you are logged in to your mailbox via Outlook Web App. You will be prompted to login if you are not already logged in with the browser.This URL can be accessed from within an iFrame.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "webLink", Required = Newtonsoft.Json.Required.Default)]
        public string WebLink { get; set; }
    
        /// <summary>
        /// Gets or sets online meeting url.
        /// A URL for an online meeting. The property is set only when an organizer specifies an event as an online meeting such as a Skype meeting. Read-only.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "onlineMeetingUrl", Required = Newtonsoft.Json.Required.Default)]
        public string OnlineMeetingUrl { get; set; }

        /// <summary>
        /// Gets or sets Images Associated with the Event.
        /// A list of images that are uploaded to the Event
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "ExtensionEventImages", Required = Newtonsoft.Json.Required.Default)]
        public ICollection<ExtensionEventImage> ExtensionEventImages {get;set;}

        /// <summary>
        /// Gets or sets created date time.
        /// The Timestamp type represents date and time information using ISO 8601 format and is always in UTC time. For example, midnight UTC on Jan 1, 2014 would look like this: '2014-01-01T00:00:00Z'
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "createdDateTime", Required = Newtonsoft.Json.Required.Default)]
        public DateTimeOffset? CreatedDateTime { get; set; }
    
        /// <summary>
        /// Gets or sets last modified date time.
        /// The Timestamp type represents date and time information using ISO 8601 format and is always in UTC time. For example, midnight UTC on Jan 1, 2014 would look like this: '2014-01-01T00:00:00Z'
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "lastModifiedDateTime", Required = Newtonsoft.Json.Required.Default)]
        public DateTimeOffset? LastModifiedDateTime { get; set; }

        public string DiscriminatorValue {
            get {
                return this.GetType().Name;
            }
        }

    
    }
}

