namespace Kers.Models.Entities.KERScore
{
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// The enum LocationUniqueIdType.
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    public enum ExtensionEventLocationUniqueIdType
    {
    
        /// <summary>
        /// unknown
        /// </summary>
        Unknown = 0,
	
        /// <summary>
        /// location Store
        /// </summary>
        LocationStore = 1,
	
        /// <summary>
        /// directory
        /// </summary>
        Directory = 2,
	
        /// <summary>
        /// private
        /// </summary>
        Private = 3,
	
        /// <summary>
        /// bing
        /// </summary>
        Bing = 4,

        /// <summary>
        /// bing
        /// </summary>
        PlanningUnit = 5,
	
    }
}