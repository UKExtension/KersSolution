import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { MessageTemplate } from './message-template';
import { EmailService } from './email.service';

@Component({
  selector: '[email-template-detail]',
  templateUrl: './email-template-detail.component.html',
  styles: []
})
export class EmailTemplateDetailComponent implements OnInit {
  
  @Input('email-template-detail') template:MessageTemplate;
  @Output() onEdited = new EventEmitter<MessageTemplate>();
  @Output() onDeleted = new EventEmitter<MessageTemplate>();
  
  default = true;
  edit = false;
  delete = false;
  constructor(
    private service: EmailService
  ) { }

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
  cancelled(){
    this.defaultView();
  }
  submitted(event:MessageTemplate){
    this.onEdited.emit(event);
  }
  confirmDelete(event:MessageTemplate){
    this.service.delete(this.template.id).subscribe(
      _ => {
        this.onDeleted.emit(this.template);
        this.defaultView();
      } 
    );
    
  }

}
