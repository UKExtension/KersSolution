import { Component, OnInit, Input } from '@angular/core';
import { DistrictService, District, EmployeeNumActivities } from './district.service';
import { Observable } from 'rxjs';
import { ExtensionArea, ExtensionRegion } from '../state/state.service';
import { PlanningUnit } from '../plansofwork/plansofwork.service';

@Component({
  selector: 'district-employees',
  templateUrl: './district-employees.component.html',
  styles: []
})
export class DistrictEmployeesComponent implements OnInit {

  @Input() district:District;
  @Input() area:ExtensionArea;
  @Input() region:ExtensionRegion;
  @Input() county:PlanningUnit;

  activities:Observable<EmployeeNumActivities[]>;
  month:number;
  year:number;
  type:string = "activity";
  skip:number = 0;
  take:number = 21;
  order:string = "asc";

  constructor(
    private service:DistrictService
  ) { }

  ngOnInit() {

  }

  switchType( type:string){
    this.type = type;
    this.resetActivites();
  }
  switchOrder(order:string){
    this.order = order;
    this.resetActivites();
  }


  onMonthSwitched(event:Date){
    this.month = event.getMonth();
    this.year = event.getFullYear();
    this.resetActivites();
  }

  loadMore(){
    this.take += 21;
    this.resetActivites();
  }

  resetActivites(){
    if( this.area != null ){
      this.activities = this.service.employeeactivity(this.area.id, this.month, this.year, this.order, this.type, this.skip, this.take, "area", true);
    }else if( this.region != null ){
      this.activities = this.service.employeeactivity(this.region.id, this.month, this.year, this.order, this.type, this.skip, this.take, "region");
    }else if( this.county != null ){
      this.activities = this.service.employeeactivity(this.county.id, this.month, this.year, this.order, this.type, this.skip, this.take, "county");
    }else{
      this.activities = this.service.employeeactivity(this.district.id, this.month, this.year, this.order, this.type, this.skip, this.take);
    }
    
  }

}
