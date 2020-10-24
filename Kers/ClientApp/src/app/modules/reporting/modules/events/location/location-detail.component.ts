import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ExtensionEventLocation } from '../extension-event';
import { ExtensionEventLocationConnection, LocationService } from './location.service';

@Component({
  selector: 'location-detail',
  templateUrl: './location-detail.component.html',
  styles: []
})
export class LocationDetailComponent implements OnInit {
  @Input() location:ExtensionEventLocationConnection;
  @Input() purpose:string = "CountyEvents";
  @Output() onSelected = new EventEmitter<ExtensionEventLocationConnection>();
  @Output() onDeleted = new EventEmitter<ExtensionEventLocationConnection>();
  
  isEditing:boolean = false;
  isDeleting:boolean = false;
  isDefault:boolean = true;
  constructor(
    private service:LocationService
  ) { }

  ngOnInit() {
  }
  selection(){
    this.onSelected.emit( this.location);
  }
  default(){
    this.isEditing = false;
    this.isDeleting = false;
    this.isDefault = true;
  }
  edit(){
    this.isEditing = true;
    this.isDeleting = false;
    this.isDefault = false;
  }
  delete(){
    this.isEditing = false;
    this.isDeleting = true;
    this.isDefault = false;
  }
  changed(event:ExtensionEventLocationConnection){
    this.isEditing = false;
    this.isDeleting = false;
    this.isDefault = true;
  }
  confirmDelete(){
    this.default();
    this.service.deleteLocationConnection(this.location.id).subscribe(
      _ => {
        this.onDeleted.emit(this.location);
      }
    )
  }

}
