namespace Kers.Models.Entities.KERScore
{
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// The enum RecurrenceRangeType.
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    public enum ExtensionEventRecurrenceRangeType
    {
    
        /// <summary>
        /// end Date
        /// </summary>
        EndDate = 0,
	
        /// <summary>
        /// no End
        /// </summary>
        NoEnd = 1,
	
        /// <summary>
        /// numbered
        /// </summary>
        Numbered = 2,
	
    }
}