import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CountyEvent, CountyEventService } from './county-event.service';

@Component({
  selector: 'county-events-convert-item',
  templateUrl: './county-events-convert-item.component.html',
  styles: []
})
export class CountyEventConvertItemComponent implements OnInit {
  @Input('service') s:Object;

  @Output() onLoaded = new EventEmitter<CountyEventConvertItemComponent>();
  @Output() onConverted = new EventEmitter<CountyEvent>();

  loading = false;

  evnt:CountyEvent;

  constructor(
    private service:CountyEventService
  ) { }

  ngOnInit() {
    this.onLoaded.emit(this);
  }
  convert(){
    this.loading = true;
    this.service.migrate(+this.s["rID"]).subscribe(
      res => {
        this.evnt = res;
        this.loading = false;
        this.onConverted.emit(this.evnt);
      }
    );
  }

}
