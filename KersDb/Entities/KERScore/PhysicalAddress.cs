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
    /// The type PhysicalAddress.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public partial class PhysicalAddress
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets street.
        /// The street.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "street", Required = Newtonsoft.Json.Required.Default)]
        public string Street { get; set; }
    
        /// <summary>
        /// Gets or sets city.
        /// The city.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "city", Required = Newtonsoft.Json.Required.Default)]
        public string City { get; set; }
    
        /// <summary>
        /// Gets or sets state.
        /// The state.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "state", Required = Newtonsoft.Json.Required.Default)]
        public string State { get; set; }
    
        /// <summary>
        /// Gets or sets countryOrRegion.
        /// The country or region. It's a free-format string value, for example, 'United States'.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "countryOrRegion", Required = Newtonsoft.Json.Required.Default)]
        public string CountryOrRegion { get; set; }
    
        /// <summary>
        /// Gets or sets postalCode.
        /// The postal code.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "postalCode", Required = Newtonsoft.Json.Required.Default)]
        public string PostalCode { get; set; }
    
    }
}
