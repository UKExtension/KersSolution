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
      <location-form *ngIf="newLocation && purpose=='CountyEvents'" [county]="county" [user]="user" (onFormCancel)="newLocation=false" (onFormSubmit)="newLocationSubmitted($event)"></location-form>
      <location-form 
              *ngIf="newLocation && purpose=='Mileage'" 
                      [showZip]="showZip" 
                      [isItBuilding]="false"
                      [showState]="true"
                      [isNameRequired]="true"
                      [isCityRequired]="true"
                      [isStateRequired]="true"
                      [showDisplayName]="true"
                      [county]="county"  
                      [user]="user" 
                          (onFormCancel)="newLocation=false" 
                          (onFormSubmit)="newLocationSubmitted($event)">
      </location-form>
      <br><br>
      <div class="row">
          <div class="col-sm-6 col-xs-12">
            <input type="text" [(ngModel)]="search" placeholder="search" (keyup)="onSearch($event)" class="form-control" name="Search" />
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
          <location-detail [showZip]="showZip" [location]="locationConnection" [purpose]="purpose" (onSelected)="locationSelected($event)" (onDeleted)="deleted($event)"></location-detail>
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
  @Input() includeCountyOffice:boolean = false;
  @Input() purpose:string = "CountyEvents";
  @Input() showZip:boolean = false;

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

    this.refresh = new Subject();

    this.countyLocations$ = this.refresh.asObservable()
      .pipe(
        startWith('onInit'), // Emit value to force load on page load; actual value does not matter
        flatMap(_ => {
          if(this.user != null ){
            return this.service.locationsByUser( this.user.id,this.skip,this.take, this.order, this.search, this.includeCountyOffice);
          }else{
            return this.service.locationsByCounty(( this.county ? this.county.id : 0),this.skip,this.take, this.order, this.search, this.includeCountyOffice);
          }
        }), // Get some items
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
    if(event.id != null && event.id!=0) this.service.selected(event.id).subscribe();
    this.onSelected.emit(event);
  }

  deleted(_:ExtensionEventLocation){
    //this.countyLocations$ = this.service.locationsByCounty(( this.county ? this.county.id : 0));
    this.onRefresh();
  }

}