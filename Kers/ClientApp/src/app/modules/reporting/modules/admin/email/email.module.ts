import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { EmailHomeComponent } from './email-home.component';
import { EmailRoutingModule } from './email-routing.module';
import { EmailService } from './email.service';




@NgModule({
  imports:      [ SharedModule,
                  EmailRoutingModule 
                ],
  declarations: [ EmailHomeComponent ],
  providers:    [ EmailService ]
})
export class EmailModule { }