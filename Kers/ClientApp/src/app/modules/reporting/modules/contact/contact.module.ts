import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { AngularMyDatePickerModule } from 'angular-mydatepicker';
import {ContactRoutingModule} from './contact-routing.module';
import {ActivityService} from '../activity/activity.service';

import {ContactComponent} from './contact.component';
import {ContactHomeComponent} from './contact-home.component';
import {ContactListComponent} from './contact-list.component';
import {ContactFormComponent} from './contact-form.component';
import {ContactMonthComponent} from './contact-month.component';
import {ContactDetailComponent} from './contact-detail.component';

import {ContactStatsHomeComponent} from './stats/contact-stats-home.component';
import {ContactStatsAllComponent} from './stats/contact-stats-all.component';
import {ContactStatsRow} from './stats/contact-stats-row.component';
import {ContactStatsMonthComponent} from './stats/contact-stats-month.component';
import {ContactStatsMonthRow} from './stats/contact-stats-month-row.component';
import {ContactStatsProgramComponent} from './stats/contact-stats-program.component';
import {ContactStatsProgramhRow} from './stats/contact-stats-program-row.component';


import {ContactService} from './contact.service';
 


@NgModule({
  imports:      [ SharedModule,
                  AngularMyDatePickerModule,
                  ContactRoutingModule
                  
                ],
  declarations: [ 
                    ContactComponent,
                    ContactHomeComponent,
                    ContactListComponent,
                    ContactFormComponent,
                    ContactMonthComponent,
                    ContactDetailComponent,
                    ContactStatsHomeComponent,
                    ContactStatsAllComponent,
                    ContactStatsRow,
                    ContactStatsMonthComponent,
                    ContactStatsMonthRow,
                    ContactStatsProgramComponent,
                    ContactStatsProgramhRow
                 
                ],
  providers:    [  
                    ContactService,
                    ActivityService
                ]
})
export class ContactModule { }