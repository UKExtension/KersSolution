import { Injectable } from '@angular/core';
import { PlanningUnit, User } from '../../user/user.service';

@Injectable()
export class VehicleService {

  constructor() { }
  id(id:number){
    
  }

}


export interface Vehicle{
  id:number,
  planningUnitId:number,
  planningUnit:PlanningUnit,
  addedById:number,
  addedBy:User,
  make:string,
  model:string,
  year:string,
  licenseTag:string,
  odometer:number,
  color:string,
  enabled:boolean,
  comments:string,
  datePurchesed:Date,
  dateDisposed:Date
}