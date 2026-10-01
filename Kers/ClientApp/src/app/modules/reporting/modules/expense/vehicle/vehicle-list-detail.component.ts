import { Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { Vehicle } from './vehicle.service';

@Component({
    selector: 'vehicle-list-detail',
    template: `
  <div class="ln_solid"></div>
  <div class="row">
    @if (rowDefault) {
      <div class="media event col-xs-9">
        <div class="media-body">
          <a class="title" [ngStyle]="{ 'color' : (vehicle.enabled)? 'rgb(35, 82, 124);' : '#ccc' }">{{vehicle.year}} {{vehicle.make}}@if (vehicle.name != undefined && vehicle.name != '') {
            <span> ({{vehicle.name}})</span>
          }</a>
          <p [ngStyle]="{ 'color' : (vehicle.enabled)? 'rgb(115, 135, 156)' : '#ccc' }">{{vehicle.model}}</p>
        </div>
      </div>
    }
    <div class="col-xs-3 text-right" [ngClass]="{'col-xs-3':rowDefault, 'col-xs-12':!rowDefault}">
      @if (rowDefault && !trips) {
        <a class="btn btn-info btn-xs" (click)="edit()">edit</a>
      }
      @if (rowDefault && trips) {
        <a class="btn btn-info btn-xs" (click)="edit()">trips</a>
      }
      @if (!rowDefault) {
        <a class="btn btn-info btn-xs" (click)="default()">close</a>
      }
    </div>
  </div>
  <div class="row">
    <div [ngClass]="{ 'col-xs-9': !trips, 'col-xs-12': trips }">
      @if (rowEdit && !trips) {
        <div class="col-xs-12">
          <vehicle-form [vehicle]="vehicle" (onFormCancel)="default()" (onFormSubmit)="submitted($event)"></vehicle-form>
        </div>
      }
      @if (rowEdit && trips) {
        <div class="col-xs-12">
          <vehicle-reports [vehicle]="vehicle"></vehicle-reports>
        </div>
      }
  
    </div>
  
  </div>
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
