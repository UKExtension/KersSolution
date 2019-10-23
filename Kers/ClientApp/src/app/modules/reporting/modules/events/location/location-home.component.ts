import { Component, OnInit } from '@angular/core';
import { ExtensionEventLocation } from '../extension-event';

@Component({
  selector: 'location-browser',
  template: `
  <div class="text-right">
      <a class="btn btn-info btn-xs" *ngIf="!newLocation" (click)="newLocation = true">+ new location</a>
  </div>
  <location-form *ngIf="newLocation" (onFormCancel)="newLocation=false" (onFormSubmit)="newLocationSubmitted($event)"></location-form>
  `,
  styles: []
})
export class LocationHomeComponent implements OnInit {

  newLocation = false;
  constructor() { }

  ngOnInit() {
  }
  newLocationSubmitted(event:ExtensionEventLocation){
    console.log(event);
    this.newLocation = false;
  }

}
