import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Vehicle } from './vehicle.service';

@Component({
  selector: 'vehicle-list-detail',
  template: `
  <div class="ln_solid"></div>
  <div class="row">
        <div class="media event col-xs-9" *ngIf="rowDefault">
              <div class="media-body">
              <a class="title" [ngStyle]="{ 'color' : (vehicle.enabled)? 'rgb(35, 82, 124);' : '#ccc' }">{{vehicle.year}} {{vehicle.make}}<span *ngIf="vehicle.name != undefined && vehicle.name != ''"> ({{vehicle.name}})</span></a>
              <p [ngStyle]="{ 'color' : (vehicle.enabled)? 'rgb(115, 135, 156)' : '#ccc' }">{{vehicle.model}}</p>
              </div>
       </div>
       <div class="col-xs-3 text-right" [ngClass]="{'col-xs-3':rowDefault, 'col-xs-12':!rowDefault}">
          <a class="btn btn-info btn-xs" (click)="edit()" *ngIf="rowDefault && !trips">edit</a>
          <a class="btn btn-info btn-xs" (click)="edit()" *ngIf="rowDefault && trips">trips</a>
          <a class="btn btn-info btn-xs" (click)="default()" *ngIf="!rowDefault">close</a>
      </div>  
    </div>
    <div class="row">
      <div [ngClass]="{ 'col-xs-9': !trips, 'col-xs-12': trips }">
          <div class="col-xs-12" *ngIf="rowEdit && !trips">
              <vehicle-form [vehicle]="vehicle" (onFormCancel)="default()" (onFormSubmit)="submitted($event)"></vehicle-form>
          </div>
          <div class="col-xs-12" *ngIf="rowEdit && trips">
              <vehicle-reports [vehicle]="vehicle"></vehicle-reports>
          </div>
          
      </div>
      
  </div>
  `
})
export class VehicleListDetailComponent implements OnInit {
  @Input() vehicle:Vehicle;
  @Input() trips:boolean = false;
  @Output() onEdited = new EventEmitter<Vehicle>();
  
  rowDefault =true;
  rowEdit = false;
  
  constructor() { }

  ngOnInit() {
  }
  edit(){
    this.rowDefault = false;
    this.rowEdit = true;
  }
  
  default(){
      this.rowDefault = true;
      this.rowEdit = false;
  }

  submitted(vehicle:Vehicle){
      this.vehicle = vehicle;
      this.onEdited.emit(vehicle);
      this.default();
  }       

}
