import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import {Location} from '@angular/common';
import {Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { AuthHttp } from '../../../authentication/auth.http';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import { User } from '../user/user.service';

@Injectable()
export class SnapEdCommitmentService {
  baseUrl = '/api/snapedcommitment/';

  constructor(
      private http:AuthHttp, 
      private location:Location
  ) { }


  getSnapCommitments(userid:number = 0, fisclyearid = 0):Observable<CommitmentBundle>{
    var url = this.baseUrl + 'commitments/'+ fisclyearid + '/' + userid;
    return this.http.get(this.location.prepareExternalUrl(url))
            .map(res => <CommitmentBundle>res.json())
            .catch(this.handleError);
  }


  getRequestOptions(){
    return new RequestOptions(
        {
            headers: new Headers({
                "Content-Type": "application/json; charset=utf-8"
            })
        }
    )
  }

  handleError(err:Response){
    console.error(err);
    return Observable.throw(err.json().error || 'Server error');
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

}