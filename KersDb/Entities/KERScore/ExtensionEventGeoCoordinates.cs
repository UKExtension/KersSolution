namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// The type ExtensionEventGeoCoordinates.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ExtensionEventGeoCoordinates
    {

        /// <summary>
        /// Gets or sets altitude.
        /// The altitude of the location.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "altitude", Required = Newtonsoft.Json.Required.Default)]
        public double? Altitude { get; set; }
    
        /// <summary>
        /// Gets or sets latitude.
        /// The latitude of the location.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "latitude", Required = Newtonsoft.Json.Required.Default)]
        public double? Latitude { get; set; }
    
        /// <summary>
        /// Gets or sets longitude.
        /// The longitude of the location.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "longitude", Required = Newtonsoft.Json.Required.Default)]
        public double? Longitude { get; set; }
    
        /// <summary>
        /// Gets or sets accuracy.
        /// The accuracy of the latitude and longitude. As an example, the accuracy can be measured in meters, such as the latitude and longitude are accurate to within 50 meters.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "accuracy", Required = Newtonsoft.Json.Required.Default)]
        public double? Accuracy { get; set; }
    
        /// <summary>
        /// Gets or sets altitudeAccuracy.
        /// The accuracy of the altitude.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "altitudeAccuracy", Required = Newtonsoft.Json.Required.Default)]
        public double? AltitudeAccuracy { get; set; }
    
    }
}
