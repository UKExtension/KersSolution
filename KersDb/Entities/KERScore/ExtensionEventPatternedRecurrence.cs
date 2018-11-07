namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// The type PatternedRecurrence.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ExtensionEventPatternedRecurrence
    {

        /// <summary>
        /// Gets or sets pattern.
        /// The frequency of an event.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "pattern", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventRecurrencePattern Pattern { get; set; }
    
        /// <summary>
        /// Gets or sets range.
        /// The duration of an event.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "range", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventRecurrenceRange Range { get; set; }
    
    }
}