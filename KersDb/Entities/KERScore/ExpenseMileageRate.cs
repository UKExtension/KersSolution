using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.KERScore
{

    public partial class ExpenseMileageRate : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int InstitutionId {get; set;}
        public Institution Institution {get; set;}
        public DateTime Start {get;set;}
        public DateTime End {get;set;}
        public String Description {get; set;}
        public float Rate {get;set;}
    }
}

/*
rID	instID   	dtBegin	    dtEnd	mileageRate
5	21000-1862	20150101	20151231	0.575
6	21000-1862	20160101	20161231	0.54
1	21000-1862	20110101	20121231	0.555
2	21000-1890	20060101	99991231	0.45
3	21000-1862	20130101	20131231	0.565
4	21000-1862	20140101	20141231	0.56
7	21000-1862	20170101	99991231	0.535

 */