namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// The enum DayOfWeek.
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    [Flags]
    public enum ExtensionEventDayOfWeek
    {
    
        /// <summary>
        /// sunday
        /// </summary>
        Sunday = 0,
	
        /// <summary>
        /// monday
        /// </summary>
        Monday = 1,
	
        /// <summary>
        /// tuesday
        /// </summary>
        Tuesday = 2,
	
        /// <summary>
        /// wednesday
        /// </summary>
        Wednesday = 3,
	
        /// <summary>
        /// thursday
        /// </summary>
        Thursday = 4,
	
        /// <summary>
        /// friday
        /// </summary>
        Friday = 5,
	
        /// <summary>
        /// saturday
        /// </summary>
        Saturday = 6,
	
    }
}