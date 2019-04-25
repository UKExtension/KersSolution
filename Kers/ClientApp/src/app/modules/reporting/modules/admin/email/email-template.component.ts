import { Component, OnInit } from '@angular/core';
import { MessageTemplate } from './message-template';
import { Observable } from 'rxjs';
import { EmailService } from './email.service';

@Component({
  selector: 'email-template',
  templateUrl: './email-template.component.html',
  styles: []
})
export class EmailTemplateComponent implements OnInit {
  newTemplate = false;
  loading = false;
  templates:Observable<MessageTemplate[]>;
  
  constructor(
    private service:EmailService
  ) { }

  ngOnInit() {
    this.templates = this.service.gettemplates();
  }

  newTemplateOpen(){
    this.newTemplate = true;
  }
  cancelled(){
    this.newTemplate = false;
  }
  submitted(){
    this.newTemplate = false;
    this.templates = this.service.gettemplates();
  }

}
