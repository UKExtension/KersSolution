import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { EmailService } from './email.service';
import { MessageTemplate } from './message-template';

@Component({
  selector: 'email-template-form',
  templateUrl: './email-template-form.component.html',
  styles: []
})
export class EmailTemplateFormComponent implements OnInit {
  @Input() template:MessageTemplate;
  templateForm:any;
  loading = false;

  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<MessageTemplate>();

  constructor(
    private fb:FormBuilder,
    private service: EmailService
  ) { 
    this.templateForm = this.fb.group(
      {
          code: ["", Validators.required],
          subject: ["", Validators.required],
          bodyHtml: [""],
          bodyText: [""] 
      });
  }

  ngOnInit() {
    if(this.template !=  null){
      this.templateForm.patchValue(this.template);
    }
  }

  onCancel(){
    this.onFormCancel.emit();
  }


  onSubmit(){
    this.loading = true;
    if(this.template == null){
      this.service.add(this.templateForm.value).subscribe(
        res=>{
          this.loading = false;
          this.onFormSubmit.emit(<MessageTemplate>res);
        }
      )
    }else{
      this.service.update(this.template.id, this.templateForm.value).subscribe(
        res=>{
          this.loading = false;
          this.onFormSubmit.emit(<MessageTemplate>res);
        }
      )
    }
  }

}
