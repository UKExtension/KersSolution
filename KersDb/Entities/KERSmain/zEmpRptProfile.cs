using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kers.Models.Entities.KERScore;

namespace Kers.Models.Entities.KERSmain
{

    [Table("zEmpRptProfile")]
    public partial class zEmpRptProfile:IEntityBase
    {
        

        [Key]
        [Column("rID")]
        public int Id { get; set; }

        public DateTime? rDT { get; set; }

        public bool? enabled { get; set; }

        public bool? extensionIntern { get; set; }

        [StringLength(50)]
        public string instID { get; set; }

        [StringLength(50)]
        public string planningUnitID { get; set; }
        public zzPlanningUnit zzPlaningUnit { get; set; }
        [StringLength(300)]
        public string planningUnitName { get; set; }

        [StringLength(50)]
        public string positionID { get; set; }

        public zzExtensionPosition zzExtensionPosition { get; set; }
        public bool? progANR { get; set; }

        public bool? progHORT { get; set; }

        public bool? progFCS { get; set; }

        public bool? prog4HYD { get; set; }

        public bool? progFACLD { get; set; }

        public bool? progNEP { get; set; }

        public bool? progOther { get; set; }

        [StringLength(50)]
        public string locationID { get; set; }
        public zzGeneralLocation zzGeneralLocation {get;set;}

        [StringLength(50)]
        public string linkBlueID { get; set; }

        [StringLength(8)]
        public string personID { get; set; }

        [StringLength(300)]
        public string personName { get; set; }

        public bool? isDD { get; set; }

        public bool? isCesInServiceTrainer { get; set; }

        public bool? isCesInServiceAdmin { get; set; }

        [StringLength(300)]
        public string emailDeliveryAddress { get; set; }

        [StringLength(300)]
        public string emailUEA { get; set; }

    }
}
