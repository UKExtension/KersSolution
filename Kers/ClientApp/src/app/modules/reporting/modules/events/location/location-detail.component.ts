import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ExtensionEventLocation } from '../extension-event';

@Component({
  selector: 'location-detail',
  templateUrl: './location-detail.component.html',
  styles: []
})
export class LocationDetailComponent implements OnInit {
  @Input() location:ExtensionEventLocation;
  @Output() onSelected = new EventEmitter<ExtensionEventLocation>();

  constructor() { }

  ngOnInit() {
  }
  selection(){
    this.onSelected.emit( this.location);
  }
  edit(){

  }
  delete(){

  }
  confirmDelete(){
    
  }

}
