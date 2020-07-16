import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { CountyEvent, CountyEventService, CountyEventWithTime } from './county-event.service';

@Component({
  selector: 'app-county-events-home',
  template: `
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newEvent" (click)="newEvent = true">+ new event</a>
    </div>
    <county-event-form *ngIf="newEvent" (onFormCancel)="newEvent = false" (onFormSubmit)="newEventSubmitted($event)"></county-event-form>
    <br><br>
    <table class="table table-bordered table-striped">
      <tr [county-event-list-details]="ev" *ngFor="let ev of events | async" (onDeleted)="newEventSubmitted($event)"></tr>
    </table>
  `,
  styles: []
})
export class CountyEventsHomeComponent implements OnInit {

  events:Observable<CountyEventWithTime[]>;
  newEvent = false;

  constructor(
    private service:CountyEventService
  ) { }

  ngOnInit() {
    this.events = this.service.range();
  }

  newEventSubmitted(_:CountyEvent){
    this.newEvent = false;
    this.events = this.service.range();
  }

}
