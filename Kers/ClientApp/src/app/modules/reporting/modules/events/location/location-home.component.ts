import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtensionEventLocation } from '../extension-event';
import { PlanningUnit } from '../../plansofwork/plansofwork.service';
import { User } from '../../user/user.service';
import { LocationService, ExtensionEventLocationConnection, ExtensionEventLocationConnectionSearchResult,  } from './location.service';
import { Observable, Subject } from 'rxjs';
import { startWith, flatMap, tap } from 'rxjs/operators';

@Component({
  selector: 'location-browser',
  template: `
  <div *ngIf="!user && !county">No county or user provided</div>
  <div *ngIf="user || county" style="background-color: #F8F8F8; border-top: 1px solid #eee; border-bottom: 1px solid #eee; padding: 12px 7px; ">
    <div *ngIf="countyLocations$ | async as countyLocations">
      <div class="text-right">
          <a class="btn btn-info btn-xs" *ngIf="!newLocation" (click)="newLocation = true">+ new location</a>
      </div>
      <location-form *ngIf="newLocation" [county]="county" [user]="user" (onFormCancel)="newLocation=false" (onFormSubmit)="newLocationSubmitted($event)"></location-form>
      <br><br>
      <div class="row">
          <div class="col-sm-6 col-xs-12">
            <input type="text" [(ngModel)]="search" placeholder="search by building name" (keyup)="onSearch($event)" class="form-control" name="Search" />
          </div>
          <div class="col-sm-6 col-xs-12 text-right">

Order by:&nbsp; 
            <div class="btn-group" data-toggle="buttons">
              <label class="btn btn-default" [class.active]="order=='often'">
                <input type="radio" name="type" id="option2" (click)="switchOrder('often')"> Often Used
              </label>
              <label class="btn btn-default" [class.active]="order=='asc'">
                <input type="radio" name="type" id="option3" (click)="switchOrder('asc')"> Name
              </label>
            </div>



        </div>
      </div>
      <loading *ngIf="loading"></loading>      
      <div class="row" *ngIf="!loading">
        <div *ngFor="let locationConnection of countyLocations.results">
          <location-detail [location]="locationConnection" (onSelected)="locationSelected($event)" (onDeleted)="deleted($event)"></location-detail>
        </div>
        <div class="col-xs-12"><br><br>
          <div *ngIf="countyLocations.count != 0" class="text-center">
            <div>Showing {{ ((skip + take) < countyLocations.count ? (skip + take) : countyLocations.count)}} of {{countyLocations.count}} Addresses</div>
            <div *ngIf="countyLocations.count >= (skip + take)" class="btn btn-app" style="width: 97%; margin-right: 35px;" (click)="loadMore()">
                load more <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
          </div>
        </div><br><br>
      </div>
    </div>
  </div>
  `,
  styles: []
})
export class LocationHomeComponent implements OnInit {
  @Input() county:PlanningUnit;
  @Input() user:User;

  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off

  skip:number = 0;
  take:number = 6;
  order:string = "often";
  search:string = "";

  @Output() onSelected = new EventEmitter<ExtensionEventLocationConnection>();
  @Output() onCanceled = new EventEmitter<void>();

  countyLocations$: Observable<ExtensionEventLocationConnectionSearchResult>;

  newLocation = false;
  constructor(
    private service: LocationService
  ) { }


  ngOnInit() {

    /* 
    this.criteria = {
      search: "",
      order: 'frq',
      amount: 6
    }
     */
    this.refresh = new Subject();

    this.countyLocations$ = this.refresh.asObservable()
      .pipe(
        startWith('onInit'), // Emit value to force load on page load; actual value does not matter
        flatMap(_ => this.service.locationsByCounty(( this.county ? this.county.id : 0),this.skip,this.take, this.order, this.search)), // Get some items
        tap(_ => this.loading = false) // Turn off the spinner
      );
  }
  onSearch(event){
    this.search = event.target.value;
    this.onRefresh();
  }
   

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }
 
  switchOrder(type:string){
    this.order = type;
    this.onRefresh();
  }

  loadMore(){
    this.take += 6;
    this.onRefresh();
  }


  newLocationSubmitted(event:ExtensionEventLocationConnection){
    this.newLocation = false;
    //this.countyLocations$ = this.service.locationsByCounty(( this.county ? this.county.id : 0));
    
    this.onSelected.emit(event);
    this.onRefresh();
  }
  locationSelected(event:ExtensionEventLocationConnection){
    this.service.selected(event.id).subscribe();
    this.onSelected.emit(event);
  }

  deleted(_:ExtensionEventLocation){
    this.countyLocations$ = this.service.locationsByCounty(( this.county ? this.county.id : 0));
  }

}



/*


import { Component, OnInit, Output, EventEmitter} from '@angular/core';
import { FarmerAddress, SoildataService, FarmerAddressSearchResult } from '../soildata.service';
import { Observable, Subject } from 'rxjs';
import { FarmerAddressSearchCriteria } from '../soildata.report';
import { startWith, flatMap, tap } from 'rxjs/operators';

@Component({
  selector: 'soildata-address-browser',
  template: `
    <div class="ln_solid"></div>

    <div class="row" *ngIf="addresses$ | async as addresses">
      <div class="col-xs-12" style="margin-bottom: 30px;">
        <a class="btn btn-info btn-xs pull-right" (click)="canceled()">close</a>
      </div>
      <div class="col-sm-6 col-xs-12">
        <input type="text" [(ngModel)]="criteria.search" placeholder="search by farmer name" (keyup)="onSearch($event)" class="form-control" name="Search" />
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
      <loading *ngIf="loading"></loading>
      <div *ngIf="!loading">
        <div class="col-lg-4 col-md-6 col-xs-12" *ngFor="let address of addresses.data"><br>
        {{address.first}} {{address.last}}<br>
        {{address.address}}<br>
        {{address.city}}, {{address.st}} {{address.zip}} <br><span *ngIf="address.emailAddress">Email: {{address.emailAddress}}</span><br>
        <a class="btn btn-info btn-xs" (click)="selected(address)">select</a>
        </div>
      </div>


      <div class="col-xs-12"><br><br>
        <div *ngIf="addresses.count != 0" class="text-center">
          <div>Showing {{ (criteria.amount < addresses.count ? criteria.amount : addresses.count)}} of {{addresses.count}} Addresses</div>
          <div *ngIf="addresses.count >= criteria.amount" class="btn btn-app" style="width: 97%; margin-right: 35px;" (click)="loadMore()">
              load more <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
        </div>
      </div><br><br>


    </div>

    <div class="row"><br>
      <div class="col-xs-12">
        <a class="btn btn-info btn-xs pull-right" (click)="newAddress = true" *ngIf="!newAddress">+ new address</a>
      </div>
      <div class="col-xs-12" *ngIf="newAddress">
        <div class="ln_solid"></div>
        <soildata-farmer-address-form (onFormCancel)="newAddress = false" (onFormSubmit)="selected($event)"></soildata-farmer-address-form>
      </div>
    </div>
    
    <div class="ln_solid"></div>
  `,
  styles: []
})
export class SoildataAddressBrowserComponent implements OnInit {


  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  type="frq";
  criteria:FarmerAddressSearchCriteria;
  




  
  @Output() onSelected = new EventEmitter<FarmerAddress>();
  @Output() onCanceled = new EventEmitter<void>();
  addresses$:Observable<FarmerAddressSearchResult>;
  newAddress = false;

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
  selected(address:FarmerAddress){
    this.onSelected.emit(address);
  }
  canceled(){
    this.onCanceled.emit();
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

*/