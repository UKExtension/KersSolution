import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { fstat } from 'fs';
import { User, PlanningUnit } from '../user/user.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Injectable({
  providedIn: 'root'
})
export class BudgetService {
  private baseUrl = '/api/BudgetPlan/';

  private handleError: HandleError;

  constructor(private http: HttpClient, 
    private location:Location,
    httpErrorHandler: HttpErrorHandler
    ) {
        this.handleError = httpErrorHandler.createHandleError('BudgetService');
    }

  addBudget( plan:BudgetPlanRevision ):Observable<BudgetPlanRevision>{
    return this.http.post<BudgetPlanRevision>(this.location.prepareExternalUrl(this.baseUrl+"budget"), JSON.stringify(plan))
            .pipe(
                catchError(this.handleError('add', <BudgetPlanRevision>{}))
            );
  }

  updateBudget(id:number, plan:BudgetPlanRevision):Observable<BudgetPlanRevision>{
    var url = this.baseUrl + "budget/" + id;
    return this.http.put<BudgetPlanRevision>(this.location.prepareExternalUrl(url), JSON.stringify(plan))
            .pipe(
                catchError(this.handleError('update', plan))
            );
  }

  getOfficeOperations( onlyactive:boolean = false ):Observable<BudgetPlanOfficeOperation[]>{
    var url = this.baseUrl + "officeoperations/" + onlyactive;
    return this.http.get<BudgetPlanOfficeOperation[]>(this.location.prepareExternalUrl(url))
        .pipe(
            catchError(this.handleError('getOfficeOperations', []))
        );
  }

  add( operation:BudgetPlanOfficeOperation ):Observable<BudgetPlanOfficeOperation>{
    return this.http.post<BudgetPlanOfficeOperation>(this.location.prepareExternalUrl(this.baseUrl), JSON.stringify(operation))
            .pipe(
                catchError(this.handleError('add', <BudgetPlanOfficeOperation>{}))
            );
  }

  update(id:number, operation:BudgetPlanOfficeOperation):Observable<BudgetPlanOfficeOperation>{
    var url = this.baseUrl + id;
    return this.http.put<BudgetPlanOfficeOperation>(this.location.prepareExternalUrl(url), JSON.stringify(operation))
            .pipe(
                catchError(this.handleError('update', operation))
            );
  }  
}

export class BudgetPlanOfficeOperation{
  id:number;
  name:string;
  order:number;
  active:boolean;
}

export class BudgetPlan{
  id:number;
  fiscalYear:FiscalYear;
  planningUnit:PlanningUnit;
  active:boolean;
  revisions:BudgetPlanRevision[];
  lastRevision:BudgetPlanRevision;
}

export class BudgetPlanRevision{
  id:number;
  budgetPlanId:Number;
  budgetPlan:BudgetPlan;
  kersUser:User;
  created:Date;
  realPropertyAssesment:number;
  realPropertyTaxRate:number;
  personalPropertyAssesment:number;
  personalPropertyTaxRate:number;
  motorVehicleAssesment:number;
  motorVehicleTaxRate:number;
  anticipatedDelinquency:number;
  collection:number;
  otherExtDistTaxes1:number;
  otherExtDistTaxes2:number;
  coGenFund:number;
  userDefinedIncome:BudgetPlanUserDefinedIncome[];
  interest:number;
  reserve:number;
  capitalImpFund:number;
  equipmentFund:number;
  anticipatedCarryover:number;
  budgetPlanStaffExpenditures:BudgetPlanStaffExpenditure[];
  baseAgentContribution:number;
  travelExpenses:BudgetPlanTravelExpenses[];
  professionalImprovemetnExpenses:BudgetPlanProfessionalImprovementExpenses[];
  numberOfProfessionalStaff:number;
  amontyPerProfessionalStaff:number;
  additionalOperationalCostPerPerson:number;
  ukPostage:number;
  ukPublications:number;
  capitalImprovementFundForEmergency:number;
  equipmentFundForEmergency:number;
  officeOperationValues:BudgetPlanOfficeOperationValue[];
}
export class BudgetPlanUserDefinedIncome{
  id:number;
  name:string;
  value:number;
}
export class BudgetPlanStaffExpenditure{
  id:number;
  person:User;
  personNameIfNotAUser:string;
  personId:number;
  hourlyRate:number;
  hoursPerWeek:number;
  benefitRateInPercents:number;
  expenditureType:number;
  index:number;
}
export class BudgetPlanTravelExpenses{
  id:number;
  person:User;
  personId:number;
  personNameIfNotAUser:string;
  amount:number;
  staffTypeId:number;
  staffType:BudgetPlanStaffType;
  index:number;
}
export class BudgetPlanStaffType{
  id:number;
  name: string;
  canItBeRepitted:boolean;
  active:boolean;
}
export class BudgetPlanProfessionalImprovementExpenses{
  id:number;
  person:User;
  personNameIfNotAUser:string;
  amount:number;
  staffType:BudgetPlanStaffType;
}
export class BudgetPlanOfficeOperationValue{
  id:number;
  budgetPlanOfficeOperation:BudgetPlanOfficeOperation;
  budgetPlanOfficeOperationId: number;
  value:number;
}


