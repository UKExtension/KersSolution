import {FarmerAddress} from './soildata.service';


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
}

export class TestResults{
    id:number;
    priveIndex:string;
    labNum:string;
    order:string;
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