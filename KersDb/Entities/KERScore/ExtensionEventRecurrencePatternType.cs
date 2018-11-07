namespace Kers.Models.Entities.KERScore
{
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// The enum RecurrencePatternType.
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    public enum ExtensionEventRecurrencePatternType
    {
    
        /// <summary>
        /// daily
        /// </summary>
        Daily = 0,
	
        /// <summary>
        /// weekly
        /// </summary>
        Weekly = 1,
	
        /// <summary>
        /// absolute Monthly
        /// </summary>
        AbsoluteMonthly = 2,
	
        /// <summary>
        /// relative Monthly
        /// </summary>
        RelativeMonthly = 3,
	
        /// <summary>
        /// absolute Yearly
        /// </summary>
        AbsoluteYearly = 4,
	
        /// <summary>
        /// relative Yearly
        /// </summary>
        RelativeYearly = 5,
	
    }
}