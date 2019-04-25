import { Component, OnInit, Input } from '@angular/core';
import { MessageTemplate } from './message-template';

@Component({
  selector: '[email-template-detail]',
  templateUrl: './email-template-detail.component.html',
  styles: []
})
export class EmailTemplateDetailComponent implements OnInit {
  
  @Input('email-template-detail') template:MessageTemplate;
  
  default = true;
  edit = false;
  delete = false;
  constructor() { }

  ngOnInit() {
  }


  defaultView(){
    this.default = true;
    this.edit = false;
    this.delete = false;
  }

  editView(){
    this.default = false;
    this.edit = true;
    this.delete = false;
  }

  deleteView(){
    this.default = false;
    this.edit = false;
    this.delete = true;
  }


}
