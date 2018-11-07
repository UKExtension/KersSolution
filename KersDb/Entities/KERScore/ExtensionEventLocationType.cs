namespace Kers.Models.Entities.KERScore
{
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// The enum LocationType.
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    public enum ExtensionEventLocationType
    {
    
        /// <summary>
        /// default
        /// </summary>
        Default = 0,
	
        /// <summary>
        /// conference Room
        /// </summary>
        ConferenceRoom = 1,
	
        /// <summary>
        /// home Address
        /// </summary>
        HomeAddress = 2,
	
        /// <summary>
        /// business Address
        /// </summary>
        BusinessAddress = 3,
	
        /// <summary>
        /// geo Coordinates
        /// </summary>
        GeoCoordinates = 4,
	
        /// <summary>
        /// street Address
        /// </summary>
        StreetAddress = 5,
	
        /// <summary>
        /// hotel
        /// </summary>
        Hotel = 6,
	
        /// <summary>
        /// restaurant
        /// </summary>
        Restaurant = 7,
	
        /// <summary>
        /// local Business
        /// </summary>
        LocalBusiness = 8,
	
        /// <summary>
        /// postal Address
        /// </summary>
        PostalAddress = 9,

        /// <summary>
        /// postal Address
        /// </summary>
        ExtensionOffice = 10,
	
    }
}