import { PlanningUnit } from "../plansofwork/plansofwork.service";
import { User } from "../user/user.service";

export class TaxExempt{
    id:number;
    by:User;
    byId:number;
    unit:PlanningUnit;
    unitId:number;
    name:string;
    ein:string;
    bankName:string;
    bankAccountName:string;
    donorsReceivedAck:string;
    annBudget:string;
    annFinancialRpt:string;
    annAuditRpt:string;
    annInvRpt:string;
    areas:TaxExemptArea[];
    taxExemptFinancialYear:TaxExemptFinancialYear;
    taxExemptFinancialYearId:number;
    handled:TaxExemptFundsHandled
    handledId:number;
    taxExemptProgramCategories:TaxExemptProgramCategoryConnection[];
    districtName:string;
    districtEin:string;
    organizationName:string;
    organizationEin:string;
    organizationResides:PlanningUnit;
    organizationResidesId:number;
    organizationLetterDate:string;
    organizationSignedDate:string;
    organizationAppropriate:string;
    created:Date;
    updated:Date;

    
}

export class TaxExemptArea{
    id:number;
    unit:PlanningUnit;
    unitId:number;
    taxExemptId:number;
}

export class TaxExemptFinancialYear{
    id:number;
    name: string;
    order:number;
}

export class TaxExemptFundsHandled{
    id:number;
    name:string;
    order:number;
    active:boolean;
    is501:boolean;
}
export class TaxExemptProgramCategory{
    id:number;
    name:string;
    order:number;
    active:boolean;
}
export class TaxExemptProgramCategoryConnection{
    id:number;
    taxExempt:TaxExempt;
    taxExemptId:number;
    taxExemptProgramCategoryId:number;
    taxExemptProgramCategory:TaxExemptProgramCategory;
}