import {FarmerAddress, CountyCode} from './soildata.service';


export class SoilReportBundle{
    id:number;
    uniqueCode:string;
    sampleLabelCreated:Date;
    labTestsReady:Date;
    dataProcessed:Date;
    agentReviewed:Date;
    planningUnit:CountyCode;
    farmerForReport:FarmerForReport;
    typeForm:TypeForm;
    reports:SoilReport[];
    statusHistory:SoilReportStatusChange[];
    lastStatus:SoilReportStatusChange;
}

export class SoilReportStatusChange{
    id:number;
    soilReportStatus:SoilReportStatus;
    created:Date;
}

export class SoilReportStatus{
    id:number;
    name:string;
    description:string;
    cssClass:string;
}

export class SoilReport{
    id:number;
    dateIn:Date;
    dateSent:Date;
    dateOut:Date;
    typeForm:string;
    labNum:string;
    coId:string;
    coSamnum:string;
    farmerID:string;
    osID:string;
    acres:string;
    cropInfo1:string;
    cropInfo2:string;
    cropInfo3:string;
    cropInfo4:string;
    cropInfo6:string;
    cropInfo7:string;
    cropInfo8:string;
    cropInfo9:string;
    cropInfo10:string;
    cropInfo11:string;
    comment1:string;
    comment2:string;
    comment3:string;
    comment4:string;
    comment5:string;
    comment6:string;
    comment7:string;
    limeComment:string;
    agentNote:string;
    extra1:string;
    extra2:string;
    extra3:string;
    dateTimeFromAllAccess:Date;
    status:string;
    extInfo1:string;
    extInfo2:string;
    extInfo3:string;
    extInfo4:string;
    soilReportBundle:SoilReportBundle;
}

export class TestResults{
    id:number;
    priveIndex:number;
    labNum:string;
    order:number;
    testName:string;
    unit:string;
    result:string;
    level:string;
    recommmendation:string;
    suppInfo1:string;
    suppInfo2:string;
}

export class FarmerForReport extends FarmerAddress{
    labNum:string;
}


export class FormTypeSignees{
    id:number;
    planningUnit:CountyCode;
    typeForm:TypeForm;
    typeFormId:number;
    signee:string;
    title:string;
}

export class TypeForm{
    id:number;
    code:string;
    name:string;
    note:string;
}

export class SoilReportSearchCriteria{
    start: string;
    end: string;
    search: string = "";
    status: number[];
    order:string;
    formType: number[];
}