import { Injectable } from '@angular/core';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { HttpErrorHandler, HandleError } from '../../core/services/http-error-handler.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import { User } from '../user/user.service';

@Injectable()
export class SnapEdCommitmentService {
  baseUrl = '/api/snapedcommitment/';
  commitmentFiscalYearName = "2019";
  private handleError: HandleError;


  constructor( 
      private http: HttpClient, 
      private location:Location,
      httpErrorHandler: HttpErrorHandler
      ) {
          this.handleError = httpErrorHandler.createHandleError('SnapEdCommitmentService');
      }

  commitmentFiscalYear():Observable<FiscalYear>{
    var url = this.baseUrl + 'FiscalYearByName/'+ this.commitmentFiscalYearName;
    return this.http.get<FiscalYear>(this.location.prepareExternalUrl(url))
        .pipe(
          catchError(this.handleError('commitmentFiscalYear', <FiscalYear>{}))
        );
  }

  addOrEditCommitment(commitment:CommitmentBundle):Observable<CommitmentBundle>{
    return this.http.post<CommitmentBundle>(this.location.prepareExternalUrl(this.baseUrl), commitment)
      .pipe(
        catchError(this.handleError('addOrEditCommitment', commitment))
      );
  }

  getSnapCommitments(userid:number = 0, fisclyearid:number = 0):Observable<CommitmentBundle>{
    var url = this.baseUrl + 'commitments/'+ fisclyearid + '/' + userid;
    return this.http.get<CommitmentBundle>(this.location.prepareExternalUrl(url))
      .pipe(
        catchError(this.handleError('addOrEditCommitment', <CommitmentBundle>{}))
      );
  }

  deleteSnapCommitments(userid:number = 0, fisclyear:string = "0"):Observable<{}>{
    var url = this.baseUrl + 'commitmentsDelete/'+ fisclyear + '/' + userid;
    return this.http.delete(this.location.prepareExternalUrl(url))
      .pipe(
        catchError(this.handleError('addOrEditCommitment'))
      );
  }
}

export interface SnapEdCommitment{
  id:number;
  snapEd_ActivityTypeId?:number;
  snapEd_ActivityType:SnapEdActivityType;
  snapEd_ProjectTypeId?:number;
  snpaEdProjectType:SnapEdProjectType;
  common_FiscalYearId?:number;
  fiscalYear:FiscalYear;
  kersUserId?:number;
  kersUserId1?:number
  kersUser:User;
  amount?:number;
}


export interface SnapEdActivityType{
  id:number;
  name:string;
  measurement: string;
  common_FiscalYearId?:number;
  fiscalYear:FiscalYear;
  perProject:boolean;
}

export interface SnapEdProjectType{
  id:number;
  name:string;
  common_FiscalYearId?:number;
  fiscalYear:FiscalYear;
}

export interface SnapEdReinforcementItem{
  id:number;
  name:string;
  common_FiscalYearId?:number;
  fiscalYear:FiscalYear;
}

export interface SnapEdReinforcementItemChoice{
  id:number;
  snapEd_ReinforcementItemId?:number;
  snapEd_ReinforcementItem:SnapEdReinforcementItem;
  common_FiscalYearId?:number;
  fiscalYear:FiscalYear;
  zEmpProfileId?:number;
  kersUserId?:number;
  kersUser:User;
}
export interface SnapEd_ReinforcementItemSuggestion{
  id:number;
  suggestion:string;
  zEmpProfileId?:number;
  KersUserId?:number;
  kersUser:User;
  common_FiscalYearId?:number;
  fiscalYear:FiscalYear;
}
export interface CommitmentBundle{
  commitments:SnapEdCommitment[];
  items: SnapEdReinforcementItemChoice[];
  suggestion: SnapEd_ReinforcementItemSuggestion;
  userid:number;
  fiscalyearid:number;
}