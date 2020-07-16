import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CountyEventWithTime, CountyEventService } from './county-event.service';

@Component({
  selector: '[county-event-list-details]',
  templateUrl: './county-event-list-details.component.html',
  styles: []
})
export class CountyEventListDetailsComponent implements OnInit {
  @Input('county-event-list-details') event:CountyEventWithTime;
  rowDefault = true;
  rowEdit = false;
  rowDelete = false;

  @Output() onDeleted = new EventEmitter<CountyEventWithTime>();

  constructor(
    private service:CountyEventService
  ) { }

  ngOnInit() {
  }

  default(){
    this.rowDefault = true;
    this.rowEdit = false;
    this.rowDelete = false;
  }
  
  edit(){
    this.rowDefault = false;
    this.rowEdit = true;
    this.rowDelete = false;
  }
  delete(){
    this.rowDefault = false;
    this.rowEdit = false;
    this.rowDelete = true;
  }

  confirmDelete(){
    this.service.delete(this.event.id).subscribe(
      res => this.onDeleted.emit(this.event)
    );
  }

  eventEdited(event:CountyEventWithTime){
    this.event = event;
    this.default();
  }

}
