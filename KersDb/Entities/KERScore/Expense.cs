using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{
    public partial class Expense : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int KersUserId {get;set;}
        public KersUser KersUser {get;set;}
        public int PlanningUnitId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public DateTime ExpenseDate {get;set;}
        public int? VehicleType {get;set;}
        public List<ExpenseRevision> Revisions {get;set;}
        
        public int LastRevisionId {get;set;}

        [ForeignKey("LastRevisionId")]
        public ExpenseRevision LastRevision {get;set;}
        public DateTime Created {get;set;}
        public DateTime Updated {get;set;}
    }
}