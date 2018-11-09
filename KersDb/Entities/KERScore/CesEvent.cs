namespace Kers.Models.Entities.KERScore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CesEvent:ExtensionEvent
    {
        public int classicCesEventId { get; set; }
        public KersUser By { get; set; }

        [StringLength(300)]
        public string eventLocation { get; set; }

        [StringLength(300)]
        public string eventContact { get; set; }
    }
}