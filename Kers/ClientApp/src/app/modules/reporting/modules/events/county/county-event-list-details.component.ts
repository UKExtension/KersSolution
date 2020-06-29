import { Component, OnInit, Input } from '@angular/core';
import { CountyEvent } from './county-event.service';

@Component({
  selector: '[county-event-list-details]',
  templateUrl: './county-event-list-details.component.html',
  styles: []
})
export class CountyEventListDetailsComponent implements OnInit {
  @Input('county-event-list-details') event:CountyEvent;
  rowDefault = true;
  rowEdit = false;
  rowDelete = false;

  constructor() { }

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

}
