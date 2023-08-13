import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Alert } from './Alert';
import { AlertsService } from './alerts.service';

@Component({
  selector: '[alert-list-row]',
  templateUrl: './alert-list-row.component.html',
  styles: [
  ]
})
export class AlertListRowComponent implements OnInit {
  @Input('alert-list-row') alert:Alert;
  @Output() onEdited = new EventEmitter<Alert>();
  default = true;
  editOpen = false;
  deleteOpen = false;

  constructor(
    private service:AlertsService
  ) { }

  ngOnInit(): void {
  }

  defaultView(){
    this.default = true;
    this.editOpen = false;
    this.deleteOpen = false;
  }
  editView(){
    this.default = false;
    this.editOpen = true;
    this.deleteOpen = false;
  }
  deleteView(){
    this.default = false;
    this.editOpen = false;
    this.deleteOpen = true;
  } 
  edited(event:Alert){
    this.onEdited.emit(event);
    this.defaultView();
  }
  confirmDelete(){
    this.service.delete(this.alert.id).subscribe(
      res => {
        this.onEdited.emit(this.alert);
        this.defaultView();
      }
    );

  }

}
