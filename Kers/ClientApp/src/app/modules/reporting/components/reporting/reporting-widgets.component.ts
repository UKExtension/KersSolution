import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from './reporting.service';
import {UserService, User, PlanningUnit} from '../../modules/user/user.service';

import {RolesService, Role } from '../../modules/admin/roles/roles.service';
import { PlanningunitService } from '../../modules/planningunit/planningunit.service';
import { Vehicle } from '../../modules/expense/vehicle/vehicle.service';
import { NavigationService } from '../reporting-navigation/navigation.service';



@Component({
    template: `
  @if (errorMessage) {
    <div class="alert alert-danger alert-dismissible fade in" role="alert">
      <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span>
    </button>
    <strong>Error: </strong> {{errorMessage}}
  </div>
  }
  @if (user) {
    <div class="row">
      @if (isAgent) {
        <widget-activities-agent [enabledVehicles]="enabledVehicles"></widget-activities-agent>
      }
      @if (isProgramAssistant) {
        <widget-program-assistant [enabledVehicles]="enabledVehicles"></widget-program-assistant>
      }
      @if (isStaffAssistant) {
        <widget-staff-assistant></widget-staff-assistant>
      }
      @if (isDDAssistant) {
        <widget-dd-assistant></widget-dd-assistant>
      }
      @if (isDD) {
        <widget-dd></widget-dd>
      }
      @if (isSepcialist) {
        <widget-specialist></widget-specialist>
      }
      @if (!isAny) {
        <widget-trainings></widget-trainings>
      }
      <widget-my-info [user]="user"></widget-my-info>
    </div>
  }
  
  
  
  
  
  
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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