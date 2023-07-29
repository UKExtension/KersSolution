import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FarmerAddress, SoildataService } from '../soildata.service';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { debounce, debounceTime, distinctUntilChanged, mergeMap, switchMap } from 'rxjs/operators';
import { Observable, of } from 'rxjs';

@Component({
  selector: 'soildata-farmer-address-form',
  template: `
  <loading *ngIf="loading"></loading>
  <div class="row" *ngIf="!loading">
      <div class="col-sm-offset-3 col-sm-9">
          <h2 *ngIf="!address">New Client Address</h2>
          <h2 *ngIf="address">Update Client Address</h2>
      </div>

      <form class="form-horizontal form-label-left" novalidate (ngSubmit)="onSubmit()" [formGroup]="addressForm">
          <div class="form-group">
              <label for="first" class="control-label col-md-3 col-sm-3 col-xs-12">First Name:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="first" formControlName="first" id="first" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label for="last" class="control-label col-md-3 col-sm-3 col-xs-12">Last Name:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="last" formControlName="last" id="last" class="form-control col-xs-12" />
              </div>
          </div>
          <div *ngIf="foundSameAddress$ | async">
            <label for="last" class="control-label col-md-3 col-sm-3 col-xs-12"></label>           
            <div class="col-md-9 col-sm-9 col-xs-12 red">
                Warning: Client with the same name already exists.<br><br>
            </div>
          </div>
          <div class="form-group">
              <label for="address" class="control-label col-md-3 col-sm-3 col-xs-12">Address:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="address" formControlName="address" id="address" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label for="city" class="control-label col-md-3 col-sm-3 col-xs-12">City:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="city" formControlName="city" id="city" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label for="st" class="control-label col-md-3 col-sm-3 col-xs-12">State:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="st" formControlName="st" id="first" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label for="zip" class="control-label col-md-3 col-sm-3 col-xs-12">Zip:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="zip" formControlName="zip" id="zip" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label for="homeNumber" class="control-label col-md-3 col-sm-3 col-xs-12">Phone:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="homeNumber" formControlName="homeNumber" id="homeNumberå" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group">
              <label for="emailAddress" class="control-label col-md-3 col-sm-3 col-xs-12">Email:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12">
                  <input type="text" name="emailAddress" formControlName="emailAddress" id="emailAddress" class="form-control col-xs-12" />
              </div>
          </div>
          <div class="form-group" *ngIf="address">
              <label for="emailAddress" class="control-label col-md-3 col-sm-3 col-xs-12">Reports Url:</label>           
              <div class="col-md-9 col-sm-9 col-xs-12" style="padding-top: 5px;">
                  https://kers.ca.uky.edu/core/reports/soil/customer/{{address.uniqueCode}} &nbsp;
                    <button class="btn btn-default btn-xs" [disabled]="coppied" role="button" [copy-clipboard]="'https://kers.ca.uky.edu/core/reports/soil/customer/'+address.uniqueCode" (copied)="notify($event)">
                        <i class="fa fa-clipboard"></i>&nbsp;
                        <ng-container *ngIf="!coppied">Copy</ng-container> <ng-container *ngIf="coppied">Copied</ng-container>
                    </button>
              </div>
          </div>

          <div class="ln_solid"></div>
          <div class="form-group">
              <div class="col-md-6 col-sm-6 col-xs-12 col-sm-offset-3">
                  <a class="btn btn-primary" (click)="onCancel()">Cancel</a>
                  <button type="submit" [disabled]="addressForm.invalid"  class="btn btn-success">Submit</button>
              </div>
          </div>
      </form>
  </div>
  `,
  styles: []
})
export class SoildataFarmerAddressFormComponent implements OnInit {


  @Input() address:FarmerAddress;
  addressForm:any;
  coppied = false;
  loading = false;

  foundSameAddress$:Observable<FarmerAddress | null> = null;

  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<FarmerAddress>();

  constructor(
    private fb: FormBuilder,
    private service:SoildataService
  ) { 
    this.addressForm = this.fb.group(
      { 
          first: ["", Validators.required],
          last: [""],
          address: [""],
          city: [""],
          st: [""],
          zip: [""],
          homeNumber: [""],
          emailAddress: [""]
      });

  }


  ngOnInit() {
    if(this.address) this.addressForm.patchValue(this.address);
    var first = this.addressForm.get('first') as FormControl;
    var last = this.addressForm.get('last') as FormControl;
    this.foundSameAddress$ = first.valueChanges.pipe(
                debounceTime(500),
                distinctUntilChanged(),
                switchMap(_ => this.service.checkAddress(this.addressForm.value as FarmerAddress) ),
                mergeMap( _ => last.valueChanges.pipe(
                    debounceTime(500),
                    distinctUntilChanged(),
                    switchMap(_ => this.service.checkAddress(this.addressForm.value as FarmerAddress) )), )
            );
  }
  onCancel(){
    this.onFormCancel.emit();
  }

  onSubmit(){
      if(!this.address){

        
          var newAddress:FarmerAddress = this.addressForm.value;
          this.service.addaddress( newAddress ).subscribe(
              res => {
                  this.onFormSubmit.emit(res);
              }
          );

      }else{

          var updatedAddress:FarmerAddress = this.addressForm.value;
          this.service.updateaddress(this.address.id,  updatedAddress ).subscribe(
              res => {
                  this.address = res;
                  this.onFormSubmit.emit(res);
              }
          )
      }
      
  }

/* 

  onSearch(_){
    this.service.checkAddress( this.addressForm.value as FarmerAddress).pipe(
        debounceTime(150),
        distinctUntilChanged(),

    )
  }
 */
  public notify(payload: string) {
    this.coppied = true;
  }

}
