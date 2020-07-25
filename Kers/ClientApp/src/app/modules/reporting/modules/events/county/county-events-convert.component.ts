import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { CountyEventService, CountyEvent } from './county-event.service';
import { CountyEventConvertItemComponent } from './county-events-convert-item.component';
@Component({
  selector: 'county-events-convert',
  templateUrl: './county-events-convert.component.html',
  styles: []
})
export class CountyEventConvertComponent implements OnInit {

  services$: Observable<Object[]>;

  items: CountyEventConvertItemComponent[] = [];
  evnts: CountyEvent[] = [];
  convertAllClicked = false;
  convertedItemIndex=0;
  amount = 500;

  constructor(
    private service:CountyEventService
  ) { 
    this.services$ = service.getLegacyCountyEvents(this.amount);
  }

  ngOnInit() {
  }

  addItem(item:CountyEventConvertItemComponent){
    this.items.push(item);
  }

  addTraining(evnt:CountyEvent){
    this.evnts.push(evnt);
    if(this.convertAllClicked && this.convertedItemIndex < this.items.length - 1){
      this.items[++this.convertedItemIndex].convert();
      if(this.convertedItemIndex > this.items.length - 1) this.convertAllClicked = false;
    }
  }
  convertAll(){
    this.convertAllClicked = true;
    this.items[0].convert()
  }
  reload(){
    this.items = [];
    this.evnts = [];
    this.convertAllClicked = false;
    this.convertedItemIndex=0;
    this.services$ = this.service.getLegacyCountyEvents(this.amount);
  }

}
