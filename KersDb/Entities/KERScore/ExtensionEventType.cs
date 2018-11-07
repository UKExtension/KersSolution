namespace Kers.Models.Entities.KERScore
{
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// The enum EventType.
    /// </summary>
    [JsonConverter(typeof(EnumConverter))]
    public enum ExtensionEventType
    {
    
        /// <summary>
        /// single Instance
        /// </summary>
        SingleInstance = 0,
	
        /// <summary>
        /// occurrence
        /// </summary>
        Occurrence = 1,
	
        /// <summary>
        /// exception
        /// </summary>
        Exception = 2,
	
        /// <summary>
        /// series Master
        /// </summary>
        SeriesMaster = 3,
	
    }
}