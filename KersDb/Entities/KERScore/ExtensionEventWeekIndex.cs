namespace Kers.Models.Entities.KERScore
{
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// The enum WeekIndex.
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    public enum ExtensionEventWeekIndex
    {
    
        /// <summary>
        /// first
        /// </summary>
        First = 0,
	
        /// <summary>
        /// second
        /// </summary>
        Second = 1,
	
        /// <summary>
        /// third
        /// </summary>
        Third = 2,
	
        /// <summary>
        /// fourth
        /// </summary>
        Fourth = 3,
	
        /// <summary>
        /// last
        /// </summary>
        Last = 4,
	
    }
}