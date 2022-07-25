import { SoilReportBundle, TypeForm } from "../soildata.report";

export class SampleInfoBundle{
    id:number;
    soilReportBundle:SoilReportBundle;
    soilReportBundleId:number;
    typeForm:TypeForm;
    typeFormId:number;
    purpose:Purpose
    purposeId:number;
    sampleAttributeSampleInfoBundles:SampleAttributeSampleInfoBundle[];
};

export class Purpose{
    id:number;
    name:string;
}

export class SampleAttributeType{
    id:number;
    name:string;
    typeForm:TypeForm;
    typeFormId:number;
    order:number;
}

export class SampleAttribute{
    id:number;
    name:string;
    sampleAttributeType:SampleAttributeType;
    sampleAttributeTypeId:number
}

export class SampleAttributeSampleInfoBundle{
    id:number;
    sampleInfoBundle:SampleInfoBundle;
    sampleInfoBundleId:number;
    sampleAttribute:SampleAttribute
    sampleAttributeId:number;
}

export class LabTestType{
    id:number;
    name:string;
    code:string;
}

export class LabTestTypeSoilReportBundle{
    id:number;
    soilReportBundle:SoilReportBundle;
    soilReportBundleId:number
    labTestType:LabTestType;
    labTestTypeId:number;
}

export class BillingType{
    id:number;
    name:string;
}

export class OptionalTest{
    id:number;
    code:string;
    name:string;
}