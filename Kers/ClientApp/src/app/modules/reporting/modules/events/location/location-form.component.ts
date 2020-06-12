import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FormBuilder, Validators, AbstractControl } from '@angular/forms';
import { ExtensionEventLocation } from '../extension-event';
import { PlanningUnit } from '../../plansofwork/plansofwork.service';
import { User } from '../../user/user.service';
import { LocationService, ExtensionEventLocationConnection } from './location.service';

@Component({
  selector: 'location-form',
  template: `
<loading *ngIf="loading"></loading>
<div *ngIf="!loading" class="row">
  <div class="col-sm-offset-3 col-sm-9">
      <h2 *ngIf="!location">New Location</h2>
      <h2 *ngIf="location">Update Location</h2>
      <br><br>
  </div>
  <form class="form-horizontal form-label-left" novalidate (ngSubmit)="onSubmit()" [formGroup]="locationForm">
    <div formGroupName="address">
      <div class="form-group">
          <label for="building" class="control-label col-md-3 col-sm-3 col-xs-12">Building:</label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="building" formControlName="building" id="building" class="form-control col-xs-12" />
          </div>
      </div>
      <div class="form-group">
          <label for="street" class="control-label col-md-3 col-sm-3 col-xs-12">Address:</label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="street" formControlName="street" id="street" class="form-control col-xs-12" />
          </div>
      </div>
      <div class="form-group">
          <label for="city" class="control-label col-md-3 col-sm-3 col-xs-12">City:</label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="city" formControlName="city" id="city" class="form-control col-xs-12" />
          </div>
      </div> 
      <div class="form-group">
          <label for="state" class="control-label col-md-3 col-sm-3 col-xs-12">State:</label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="state" formControlName="state" id="state" class="form-control col-xs-12" />
          </div>
      </div> 
      <div class="form-group">
          <label for="postalCode" class="control-label col-md-3 col-sm-3 col-xs-12">Postal Code:</label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="postalCode" formControlName="postalCode" id="postalCode" class="form-control col-xs-12" />
          </div>
      </div>
    </div> 
    <div class="form-group">
          <label for="postalCode" class="control-label col-md-3 col-sm-3 col-xs-12">URL:<br><small>A web address for more information (optional).</small></label>           
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" name="locationUri" formControlName="locationUri" id="locationUri" class="form-control col-xs-12" />
          </div>
    </div>
    <div class="ln_solid"></div>
    <div class="form-group">
        <div class="col-md-6 col-sm-6 col-xs-12 col-md-offset-3">
            <a class="btn btn-primary" (click)="onCancel()">Cancel</a>
            <button type="submit" [disabled]="locationForm.invalid"  class="btn btn-success">Submit</button>
        </div>
    </div>
      
  </form>
</div>
  `,
  styles: []
})
export class LocationFormComponent implements OnInit {
  @Input() county:PlanningUnit;
  @Input() user:User;
  
  @Input() location:ExtensionEventLocationConnection;
  locationForm:any;
  loading = false;

    
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<ExtensionEventLocationConnection>();

  constructor(
    private fb: FormBuilder,
    private service: LocationService
  ) {
    this.locationForm = this.fb.group(
      {
        locationUri: "",
        address: this.fb.group(
          {
              building: [""],
              street: [""],
              city: [""],
              state: [""],
              postalCode: ""
          }
        )
      }
    );
    
   }

  ngOnInit() {
    if(this.location){
      this.locationForm.patchValue(this.location.extensionEventLocation);
    }
  }
  

  onSubmit(){
    this.loading = true;
    if(this.location){
      console.log( this.location ); 
      this.location.extensionEventLocation = <ExtensionEventLocation> this.locationForm.value;
      this.service.updateLocation(this.location.id, this.location).subscribe(
        res => {
          this.onFormSubmit.emit(res);
          this.loading = false;
        }
      ) 
    }else if(this.user || this.county){
      var loc = <ExtensionEventLocation> this.locationForm.value;
      const conn:ExtensionEventLocationConnection = <ExtensionEventLocationConnection>{};
      conn.extensionEventLocation = loc;
      if( this.user ) conn.kersUserId = this.user.id;
      if( this.county) conn.planningUnitId = this.county.id;
      this.service.addLocationConnection(conn).subscribe(
        res => {
          this.onFormSubmit.emit(res);
          this.loading = false;
        }
      )
    }else{

      /* 
      this.service.addLocation(this.locationForm.value).subscribe(
        res => {
          this.onFormSubmit.emit(res);
          this.loading = false;
        }
      ) */
    }
  }
  onCancel(){
    this.onFormCancel.emit();
  }
  

}