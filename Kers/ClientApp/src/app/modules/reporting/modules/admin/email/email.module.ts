import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { EmailHomeComponent } from './email-home.component';
import { EmailRoutingModule } from './email-routing.module';
import { EmailService } from './email.service';
import { EmailTemplateComponent } from './email-template.component';
import { EmailTemplateFormComponent } from './email-template-form.component';
import { EmailTemplateDetailComponent } from './email-template-detail.component';




@NgModule({
  imports:      [ SharedModule,
                  EmailRoutingModule 
                ],
  declarations: [ EmailHomeComponent, EmailTemplateComponent, EmailTemplateFormComponent, EmailTemplateDetailComponent ],
  providers:    [ EmailService ]
})
export class EmailModule { }