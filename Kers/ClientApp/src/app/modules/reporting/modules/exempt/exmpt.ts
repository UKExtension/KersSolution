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
    units:TaxExemptArea[];
    taxExemptFinancialYear:TaxExemptFinancialYear;
    taxExemptFinancialYearId:number;
    
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