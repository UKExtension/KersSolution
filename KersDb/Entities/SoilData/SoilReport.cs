using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kers.Models.Entities.SoilData
{

    public partial class SoilReport : IEntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Prime_Index {get;set;}
        [Column("DATE_IN")]
        public DateTime DateIn {get;set;}
        [Column("DATE_SENT")]
        public DateTime DateSent {get;set;}
        [Column("DATE_OUT")]
        public DateTime DateOut {get;set;}
        [Column("TYPE_FORM")]
        public string TypeForm {get;set;}
        [Column("LAB_NUM")]
        public string LabNum {get;set;}
        [Column("CountyCodeId")]
        public int? CoId {get;set;}
        [Column("CO_SAMNUM")]
        public string CoSamnum {get;set;}
        public string FarmerID {get;set;}
        [Column("OSID")]
        public string OsId {get;set;}
        [Column("ACRES")]
        public string Acres {get;set;}
        [Column("Crop_Info1")]
        public string CropInfo1 {get;set;}
        [Column("Crop_Info2")]
        public string CropInfo2 {get;set;}
        [Column("Crop_Info3")]
        public string CropInfo3 {get;set;}
        [Column("Crop_Info4")]
        public string CropInfo4 {get;set;}
        [Column("Crop_Info5")]
        public string CropInfo5 {get;set;}
        [Column("Crop_Info6")]
        public string CropInfo6 {get;set;}
        [Column("Crop_Info7")]
        public string CropInf7 {get;set;}
        [Column("Crop_Info8")]
        public string CropInfo8 {get;set;}
        [Column("Crop_Info9")]
        public string CropInfo9 {get;set;}
        [Column("Crop_Info10")]
        public string CropInfo10 {get;set;}
        [Column("Crop_Info11")]
        public string CropInfo11 {get;set;}
        public string Comment1 {get;set;}
        public string Comment2 {get;set;}
        public string Comment3 {get;set;}
        public string Comment4 {get;set;}
        public string Comment5 {get;set;}
        public string Comment6 {get;set;}
        public string Comment7 {get;set;}
        public string LimeComment {get;set;}
        public string AgentNote {get;set;}
        public int? NoteByKersUserId {get;set;}
        [Column("EXTRA1")]
        public string Extra1 {get;set;}
        [Column("EXTRA2")]
        public string Extra2 {get;set;}
        [Column("EXTRA3")]
        public string Extra3 {get;set;}
        public DateTime DateTimeFromAllAccess {get;set;}
        public string Status {get;set;}
        public string ExtInfo1 {get;set;}
        public string ExtInfo2 {get;set;}
        public string ExtInfo3 {get;set;}
        public string ExtInfo4 {get;set;}
        public SoilReportBundle SoilReportBundle {get;set;}
        public int? SoilReportBundleId {get;set;}
    }
}