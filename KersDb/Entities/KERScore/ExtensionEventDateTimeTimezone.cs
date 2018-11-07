namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// The type DateTimeTimeZone.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ExtensionEventDateTimeTimeZone
    {

        /// <summary>
        /// Gets or sets dateTime.
        /// A single point of time in a combined date and time representation (&amp;lt;date&amp;gt;T&amp;lt;time&amp;gt;).
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "dateTime", Required = Newtonsoft.Json.Required.Default)]
        public string DateTime { get; set; }
    
        /// <summary>
        /// Gets or sets timeZone.
        /// One of the following time zone names.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "timeZone", Required = Newtonsoft.Json.Required.Default)]
        public string TimeZone { get; set; }
    
    }
}