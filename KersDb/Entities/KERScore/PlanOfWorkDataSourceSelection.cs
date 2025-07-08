using System;
using Kers.Models.Entities.KERSmain;
using Kers.Models.Abstract;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class PlanOfWorkDataSourceSelection : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public PlanOfWorkDataSource PlanOfWorkDataSource { get; set; }
        public int PlanOfWorkDataSourceId { get; set; }
        public PlanOfWorkRevision PlanOfWorkRevision { get; set; }
        public int PlanOfWorkRevisionId { get; set; }
    }
}