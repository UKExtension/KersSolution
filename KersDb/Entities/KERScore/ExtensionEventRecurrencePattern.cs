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
    /// The type RecurrencePattern.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ExtensionEventRecurrencePattern
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// The recurrence pattern type: daily, weekly, absoluteMonthly, relativeMonthly, absoluteYearly, relativeYearly. Required.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "type", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventRecurrencePatternType? Type { get; set; }
    
        /// <summary>
        /// Gets or sets interval.
        /// The number of units between occurrences, where units can be in days, weeks, months, or years, depending on the type. Required.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "interval", Required = Newtonsoft.Json.Required.Default)]
        public Int32? Interval { get; set; }
    
        /// <summary>
        /// Gets or sets month.
        /// The month in which the event occurs.  This is a number from 1 to 12.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "month", Required = Newtonsoft.Json.Required.Default)]
        public Int32? Month { get; set; }
    
        /// <summary>
        /// Gets or sets dayOfMonth.
        /// The day of the month on which the event occurs. Required if type is absoluteMonthly or absoluteYearly.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dayOfMonth", Required = Newtonsoft.Json.Required.Default)]
        public Int32? DayOfMonth { get; set; }
    
        /// <summary>
        /// Gets or sets daysOfWeek.
        /// A collection of the days of the week on which the event occurs. The possible values are: sunday, monday, tuesday, wednesday, thursday, friday, saturday. If type is relativeMonthly or relativeYearly, and daysOfWeek specifies more than one day, the event falls on the first day that satisfies the pattern.  Required if type is weekly, relativeMonthly, or relativeYearly.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "daysOfWeek", Required = Newtonsoft.Json.Required.Default)]
        //public ICollection<ExtensionEventDayOfWeek> DaysOfWeek { get; set; }
        //Instead of the collection above not supported by ef use enum flag
        // https://stackoverflow.com/questions/8447/what-does-the-flags-enum-attribute-mean-in-c
        public int DaysOfWeek { get; set; }
        
        /// <summary>
        /// Gets or sets firstDayOfWeek.
        /// The first day of the week. The possible values are: sunday, monday, tuesday, wednesday, thursday, friday, saturday. Default is sunday. Required if type is weekly.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "firstDayOfWeek", Required = Newtonsoft.Json.Required.Default)]
        public DayOfWeek? FirstDayOfWeek { get; set; }
    
        /// <summary>
        /// Gets or sets index.
        /// Specifies on which instance of the allowed days specified in daysOfsWeek the event occurs, counted from the first instance in the month. The possible values are: first, second, third, fourth, last. Default is first. Optional and used if type is relativeMonthly or relativeYearly.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "index", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventWeekIndex? Index { get; set; }
    
    }
}