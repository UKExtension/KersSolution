using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class ExpenseMealRate : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int InstitutionId {get; set;}
        public Institution Institution {get; set;}
        public int? PlanningUnitId {get;set;}
        public PlanningUnit PlanningUnit {get;set;}
        public DateTime Start {get;set;}
        public DateTime End {get;set;}
        public String Description {get; set;}
        public float BreakfastRate {get;set;}
        public float LunchRate {get;set;}
        public float DinnerRate {get;set;}
        public int Order { get; set; }
    }
}


/*

rID	instID	dtBegin	dtEnd	rateB	rateL	rateD	rateDescription	rateCombo
1	21000-1862	20110101	99991231	7.00	11.00	23.00	standard rate	NULL
2	21000-1862	20110101	99991231	8.00	12.00	26.00	Boone	NULL
3	21000-1862	20110101	99991231	9.00	13.00	29.00	Kenton	NULL
4	21000-1862	20110101	99991231	10.00	15.00	31.00	Fayette, Jefferson	NULL
5	21000-1890	20110101	99991231	9.00	10.00	20.00	standard rate	NULL
6	21000-1890	20110101	99991231	10.00	12.00	22.00	Boone, Kenton	NULL
7	21000-1890	20110101	99991231	10.00	14.00	25.00	Fayette, Jefferson	NULL


 */