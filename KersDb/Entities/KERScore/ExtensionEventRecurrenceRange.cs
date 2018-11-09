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
    /// The type RecurrenceRange.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ExtensionEventRecurrenceRange
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// The recurrence range. The possible values are: endDate, noEnd, numbered. Required.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventRecurrenceRangeType? Type { get; set; }
    
        /// <summary>
        /// Gets or sets startDate.
        /// The date to start applying the recurrence pattern. The first occurrence of the meeting may be this date or later, depending on the recurrence pattern of the event. Must be the same value as the start property of the recurring event. Required.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "startDate", Required = Newtonsoft.Json.Required.Default)]
        public DateTime StartDate { get; set; }
    
        /// <summary>
        /// Gets or sets endDate.
        /// The date to stop applying the recurrence pattern. Depending on the recurrence pattern of the event, the last occurrence of the meeting may not be this date. Required if type is endDate.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "endDate", Required = Newtonsoft.Json.Required.Default)]
        public DateTime EndDate { get; set; }
    
        /// <summary>
        /// Gets or sets recurrenceTimeZone.
        /// Time zone for the startDate and endDate properties. Optional. If not specified, the time zone of the event is used.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "recurrenceTimeZone", Required = Newtonsoft.Json.Required.Default)]
        public string RecurrenceTimeZone { get; set; }
    
        /// <summary>
        /// Gets or sets numberOfOccurrences.
        /// The number of times to repeat the event. Required and must be positive if type is numbered.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "numberOfOccurrences", Required = Newtonsoft.Json.Required.Default)]
        public Int32? NumberOfOccurrences { get; set; }
    
    }
}