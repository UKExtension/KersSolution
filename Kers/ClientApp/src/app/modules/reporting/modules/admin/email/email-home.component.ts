import { Component } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import { EmailService, Email } from './email.service';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  templateUrl: 'email-home.component.html'
})
export class EmailHomeComponent { 


    form = null;
    selectedValue = 1;
    loading = false;
    message = "";
    constructor( 
        private reportingService: ReportingService,
        private service: EmailService,
        private fb: FormBuilder, 
    )   
    {

        this.form = fb.group(
            {
                pressets: ['1'],
                server: "outlook.office365.com",
                port: 587,
                username: "",
                password: "",
                from: ['', Validators.required],
                to: ['', Validators.required],
                subject: ['', Validators.required],
                body: ['', Validators.required],
            }
        );
    }

    ngOnInit(){
        
        this.defaultTitle();
    }

    onSubmit(){  
        this.loading = true; 
        this.service.send(<Email>this.form.value).subscribe(
            res => {
                this.loading = false;
                this.message = res.body;
            },
            err => {
                console.log(err);
            }
        );
    }

    defaultTitle(){
        this.reportingService.setTitle("Email Sending Tests");
    }
    choice(event){
        this.selectedValue = event.target.value;
    }
}