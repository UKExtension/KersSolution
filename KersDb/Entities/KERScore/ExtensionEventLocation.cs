namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.IO;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;


    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class ExtensionEventLocation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets displayName.
        /// The name associated with the location.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "displayName", Required = Newtonsoft.Json.Required.Default)]
        public string DisplayName { get; set; }
    
        /// <summary>
        /// Gets or sets locationEmailAddress.
        /// Optional email address of the location.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "locationEmailAddress", Required = Newtonsoft.Json.Required.Default)]
        public string LocationEmailAddress { get; set; }
    
        /// <summary>
        /// Gets or sets address.
        /// The street address of the location.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "address", Required = Newtonsoft.Json.Required.Default)]
        public PhysicalAddress Address { get; set; }
    
        /// <summary>
        /// Gets or sets coordinates.
        /// The geographic coordinates and elevation of the location.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "coordinates", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventGeoCoordinates Coordinates { get; set; }
    
        /// <summary>
        /// Gets or sets locationUri.
        /// Optional URI representing the location.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "locationUri", Required = Newtonsoft.Json.Required.Default)]
        public string LocationUri { get; set; }
    
        /// <summary>
        /// Gets or sets locationType.
        /// The type of location. The possible values are: default, conferenceRoom, homeAddress, businessAddress,geoCoordinates, streetAddress, hotel, restaurant, localBusiness, postalAddress. Read-only.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "locationType", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventLocationType? LocationType { get; set; }
    
        /// <summary>
        /// Gets or sets uniqueId.
        /// For internal use only.
        /// </summary>
        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "uniqueId", Required = Newtonsoft.Json.Required.Default)]
        public string UniqueId { get; set; }
    
        /// <summary>
        /// Gets or sets uniqueIdType.
        /// For internal use only.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "uniqueIdType", Required = Newtonsoft.Json.Required.Default)]
        public ExtensionEventLocationUniqueIdType? UniqueIdType { get; set; }
    
    }
}
