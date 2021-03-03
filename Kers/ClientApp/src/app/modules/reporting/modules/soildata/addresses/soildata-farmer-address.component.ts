import { Component, OnInit } from '@angular/core';
import { FarmerAddress, FarmerAddressSearchResult, SoildataService } from './../soildata.service';
import { Observable, Subject } from 'rxjs';
import { FarmerAddressSearchCriteria } from '../soildata.report';
import { flatMap, startWith, tap } from 'rxjs/operators';

@Component({
  selector: 'app-soildata-farmer-address',
  template: `
    <br>
    <h3>Client Addresses</h3>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newAddress" (click)="newAddress = true">+ new address</a>
    </div>
    <soildata-farmer-address-form *ngIf="newAddress" (onFormCancel)="newAddress=false" (onFormSubmit)="newAddressSubmitted($event)"></soildata-farmer-address-form>
    <br><br>
    
    
    <div class="row" *ngIf="addresses$ | async as addresses">
      
      <div class="col-sm-6 col-xs-12">
        <input type="text" [(ngModel)]="criteria.search" placeholder="Search by client name (first OR last)" (keyup)="onSearch($event)" class="form-control" name="Search" />
      </div>
      <div class="col-sm-6 col-xs-12 text-right">


        <div class="btn-group" data-toggle="buttons">
          <label class="btn btn-default" [class.active]="type=='frq'">
            <input type="radio" name="type" id="option2" (click)="switchOrder('frq')"> Use Frequency
          </label>
          <label class="btn btn-default" [class.active]="type=='asc'">
            <input type="radio" name="type" id="option3" (click)="switchOrder('asc')"> Name
          </label>
        </div>



      </div>
      <br><br>
      <loading *ngIf="loading"></loading>
      <div *ngIf="!loading">
        <soildata-farmer-address-detail *ngFor="let address of addresses.data" [address]="address"></soildata-farmer-address-detail>
      </div>


      <div class="col-xs-12"><br><br>
        <div *ngIf="addresses.count != 0" class="text-center">
          <div>Showing {{ (criteria.amount < addresses.count ? criteria.amount : addresses.count)}} of {{addresses.count}} Addresses</div>
          <div *ngIf="addresses.count >= criteria.amount" class="btn btn-app" style="width: 97%; margin-right: 35px;" (click)="loadMore()">
              load more <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
        </div>
      </div><br><br>


    </div>
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
      `,
  styles: []
})
export class SoildataFarmerAddressComponent implements OnInit {

  addresses:Observable<FarmerAddress[]>;
  newAddress = false;

  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  type="frq";
  criteria:FarmerAddressSearchCriteria;
  




  
  addresses$:Observable<FarmerAddressSearchResult>;

  constructor(
    private service:SoildataService
  ) { }

  ngOnInit() {
    this.criteria = {
      search: "",
      order: 'frq',
      amount: 6
    }
    
    this.refresh = new Subject();

    this.addresses$ = this.refresh.asObservable()
      .pipe(
        startWith('onInit'), // Emit value to force load on page load; actual value does not matter
        flatMap(_ => this.service.getCustomAddresses(this.criteria)), // Get some items
        tap(_ => this.loading = false) // Turn off the spinner
      );
  }
  newAddressSubmitted(_:FarmerAddress){
    this.newAddress = false;
    this.onRefresh();
  }
  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }
  

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }
  
  switchOrder(type:string){
    this.type = type;
    this.criteria["order"] = type;
    this.onRefresh();
  }

  loadMore(){
    this.criteria.amount += 6;
    this.onRefresh();
  }

}
