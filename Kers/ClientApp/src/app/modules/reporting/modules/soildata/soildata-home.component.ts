import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';
import { PlanningunitService } from '../planningunit/planningunit.service';
import { Observable } from 'rxjs';
import { PlanningUnit } from '../plansofwork/plansofwork.service';
import { CountyCode, SoildataService } from './soildata.service';
import { UserService } from '../user/user.service';
import { init } from 'echarts';

@Component({
  selector: 'app-soildata-home',
  template: `

  <div *ngIf="isUserAnAdmin">
    <div class="row" *ngIf="selectedUnit">
      <div class="col-xs-12 form-group" style="margin-top: 3px; margin-bottom: 60px;">
        <label>County</label>
        <select [(ngModel)]="selectedUnit.id" class="form-control" (change)="countySelection($event.target.value)">
          <option>Select</option>
          <option *ngFor="let unit of units | async" [value]="unit.id">{{unit.name}}</option>
        </select>
      </div>
    </div>
  </div>



    <div>
      <p>
        <a class="btn btn-default"[routerLink]="['/reporting/soildata/reports']" routerLinkActive="active">Reports</a> 
        <a class="btn btn-default" [routerLink]="['/reporting/soildata/addresses']" routerLinkActive="active">Client Addresses</a> 
        <a class="btn btn-default"[routerLink]="['/reporting/soildata/notes']" routerLinkActive="active">Notes</a> 
        <a class="btn btn-default"[routerLink]="['/reporting/soildata/signees']" routerLinkActive="active">Signees</a>
      </p>
    </div>
    <router-outlet></router-outlet>
  `,
  styles: []
})
export class SoildataHomeComponent implements OnInit {


  units:Observable<PlanningUnit[]>;
  selectedUnit: PlanningUnit;
  selectedCountyInfo:CountyCode;
  isUserAnAdmin: boolean = false;

  constructor(
    private reportingService: ReportingService,
    private unitService: PlanningunitService,
    private service:SoildataService,
    private userServide: UserService
  ) { }

  ngOnInit() {
    this.defaultTitle();
    this.units = this.unitService.counties();
    if(this.service.selectedCounty == null){
      this.userServide.current().subscribe(
        res => {
          this.selectedUnit = res.rprtngProfile.planningUnit;
          this.service.countyInfo(this.selectedUnit.id).subscribe(
            res => {
              this.selectedCountyInfo = res;
              this.service.selectedCountyCode = this.selectedCountyInfo;
              this.service.selectedCountyChange.next(this.selectedCountyInfo);
            } 
          )
          
        }
      )
    }
    this.userServide.currentUserHasAnyOfTheRoles(['STLA']).subscribe(
      res => this.isUserAnAdmin = res
    )
  }
  defaultTitle(){
    this.reportingService.setTitle("Soil Testing");
    this.reportingService.setSubtitle("");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }
  countySelection(unitId:number){
    this.service.countyInfo(unitId).subscribe(
            res => {
              this.selectedCountyInfo = res;
              this.service.selectedCountyCode = this.selectedCountyInfo;
              this.service.selectedCountyChange.next(this.selectedCountyInfo);
            } 
          );    
  }

}
