using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class TestResults : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("Test_Index")]
        public int Id { get; set; }
        [Column("Prime_Index")]
        public int PrimeIndex {get;set;}
        [Column("LAB_NUM")]
        public string LabNum {get;set;}
        public int Order {get;set;}
        public string TestName {get;set;}
        public string Unit {get;set;}
        public string Result {get;set;}
        public string Level {get;set;}
        public string Recommmendation {get;set;}
        public string SuppInfo1 {get;set;}
        public string SuppInfo2 {get;set;}
    }
}