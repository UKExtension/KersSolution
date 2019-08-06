import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { fstat } from 'fs';

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
