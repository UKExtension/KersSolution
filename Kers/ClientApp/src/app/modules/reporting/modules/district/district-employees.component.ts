import { Component, OnInit, Input } from '@angular/core';
import { DistrictService, District, EmployeeNumActivities } from './district.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'district-employees',
  templateUrl: './district-employees.component.html',
  styles: []
})
export class DistrictEmployeesComponent implements OnInit {

  @Input() district:District;

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
    console.log(event);
    this.month = event.getMonth();
    this.year = event.getFullYear();
    this.resetActivites();
  }

  loadMore(){
    this.take += 21;
    this.resetActivites();
  }

  resetActivites(){
    this.activities = this.service.employeeactivity(this.district.id, this.month, this.year, this.order, this.type, this.skip, this.take);
  }

}
