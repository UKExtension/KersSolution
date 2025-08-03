import { Component, OnInit } from '@angular/core';
import {ReportingService} from './reporting.service';
import {UserService, User, PlanningUnit} from '../../modules/user/user.service';

import {RolesService, Role } from '../../modules/admin/roles/roles.service';
import { PlanningunitService } from '../../modules/planningunit/planningunit.service';
import { Vehicle } from '../../modules/expense/vehicle/vehicle.service';
import { NavigationService } from '../reporting-navigation/navigation.service';



@Component({
  template: `
  <div class="alert alert-danger alert-dismissible fade in" role="alert" *ngIf="errorMessage">
      <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span>
      </button>
      <strong>Error: </strong> {{errorMessage}}
  </div>
  <div class="row" *ngIf="user">
    <widget-activities-agent [enabledVehicles]="enabledVehicles" *ngIf="isAgent"></widget-activities-agent>
    <widget-program-assistant [enabledVehicles]="enabledVehicles" *ngIf="isProgramAssistant"></widget-program-assistant>
    <widget-staff-assistant *ngIf="isStaffAssistant"></widget-staff-assistant>
    <widget-dd-assistant *ngIf="isDDAssistant"></widget-dd-assistant>
    <widget-dd *ngIf="isDD"></widget-dd>
    <widget-specialist *ngIf="isSepcialist"></widget-specialist>

    <widget-trainings *ngIf="!isAny"></widget-trainings>

    <widget-my-info [user]="user"></widget-my-info>
    
</div>






  `
})
export class ReportingWidgetsComponent implements OnInit { 
 

  isDD = false;
  isDDAssistant = false;
  isAgent = false;
  isSepcialist = false;
  isStaffAssistant = false;
  isProgramAssistant = false;
  currentPlanningUnit:PlanningUnit;
  enabledVehicles:Vehicle[];

  isAny = false;

  roles:Role[];

  public user:User;
  errorMessage:string;

  constructor( 
        private reportingService: ReportingService,
        private userService:UserService,
        private rolesService:RolesService,
        private planningUnitService: PlanningunitService,
        private navService: NavigationService
        ) 
    {}


  ngOnInit(){


this.navService.checktoken().subscribe(
            res => {
                if(res == true){






    this.userService.current().subscribe(
      res=> { 
          
          this.user = <User>res;
          this.rolesService.listRoles().subscribe(
              roles =>  {
                this.roles = roles;
                this.check();
              },
              error =>  this.errorMessage = <any>error
          );
          this.planningUnitService.id(this.user.rprtngProfile.planningUnitId).subscribe(
            res => {
               this.currentPlanningUnit = res;
               this.enabledVehicles = this.currentPlanningUnit.vehicles.filter( v => v.enabled);
            }
        )
          
      },
      error => this.errorMessage = <any>error
    );



                }
            },
            error =>  this.errorMessage = <any>error
        )






    this.reportingService.setTitle("Welcome to Kentucky Extension Reporting System");
  }

  check(){  
    
    if(this.hasRole("DD")){
      this.isDD = true;
      this.isAny = true;
    }else if(!this.isAny && this.hasRole("DDASST")){
      this.isDDAssistant = true;
      this.isAny = true;
    }else if(!this.isAny && 
                (
                  this.user.extensionPosition.code == "AGENT"
                )
              
              ){
      this.isAgent = true;
      this.isAny = true;
    }else if(!this.isAny && 
                (
                  this.user.extensionPosition.code == "EXTPROGASSIST"
                )
              ){
      this.isProgramAssistant = true;
      this.isAny = true;
    } 

    if( !this.isAny 
        && 
        (
          this.user.extensionPosition.code == "EXTFAC"
          ||
          this.user.extensionPosition.code == "EXTSPEC"
          ||
          this.user.extensionPosition.code == "EXTASSOC"
          ||
          this.user.extensionPosition.code == "AGCSPEC"
          ||
          this.user.extensionPosition.code == "EXTPROGCOOR"
           
        )){
      this.isSepcialist = true;
      this.isAny = true;
    }
/* 
    if(!this.isAny ){
      this.isAny = true;
      this.isStaffAssistant = true;
    }
 */



    
  }
  hasRole(rl:string):boolean{
    if(this.user.roles == null){
      return false;
    }
    var r = this.user.roles.filter(r=>r.zEmpRoleTypeId == this.roleId(rl));
    if(r.length > 0){
      return true;
    }else{
      return false;
    }
  }
  roleId(rl:string){
    var r = this.roles.filter(l=>l.shortTitle == rl);
    if(r.length > 0){
      return r[0].id;
    }else{
      return 0;
    }
  }

}