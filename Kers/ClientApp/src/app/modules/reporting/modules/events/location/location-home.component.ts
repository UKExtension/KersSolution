import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ExtensionEventLocation } from '../extension-event';
import { PlanningUnit } from '../../plansofwork/plansofwork.service';
import { User } from '../../user/user.service';
import { LocationService, ExtensionEventLocationConnection,  } from './location.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'location-browser',
  template: `
  <div *ngIf="!user && !county">No county or user provided</div>
  <div *ngIf="user || county">
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newLocation" (click)="newLocation = true">+ new location</a>
    </div>
    <location-form *ngIf="newLocation" [county]="county" [user]="user" (onFormCancel)="newLocation=false" (onFormSubmit)="newLocationSubmitted($event)"></location-form>
    <div class="row">
      <div *ngFor="let locationConnection of countyLocations$ | async" class="col-md-3">
        <location-detail [location]="locationConnection.extensionEventLocation"></location-detail>
      </div>
    </div>
  </div>
  `,
  styles: []
})
export class LocationHomeComponent implements OnInit {
  @Input() county:PlanningUnit;
  @Input() user:User;

  @Output() onSelected = new EventEmitter<ExtensionEventLocation>();
  @Output() onCanceled = new EventEmitter<void>();

  countyLocations$: Observable<ExtensionEventLocationConnection[]>;

  newLocation = false;
  constructor(
    private service: LocationService
  ) { }

  ngOnInit() {
    this.countyLocations$ = this.service.locationsByCounty(( this.county ? this.county.id : 0));
  }
  newLocationSubmitted(event:ExtensionEventLocation){
    console.log(event);
    this.newLocation = false;
  }

}
