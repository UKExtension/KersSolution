import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { MyDatePickerModule } from 'mydatepicker';
import { MyDateRangePickerModule } from 'mydaterangepicker';
import { MeetingHomeComponent } from './meeting-home.component';
import { MeetingRoutingModule } from './meeting-routing.module';
import { MeetingService } from './meeting.service';
import { MeetingListComponent } from './meeting-list.component';
import { MeetingListDetailComponent } from './meeting-list-detail.component';
import { MeetingFormComponent } from './meeting-form.component';




@NgModule({
  imports:      [   SharedModule,
                    MyDatePickerModule,
                    MyDateRangePickerModule,
                    MeetingRoutingModule
                ],
  declarations: [ 
                    MeetingHomeComponent, MeetingListComponent, MeetingListDetailComponent, MeetingFormComponent

                ],
  providers:    [  
                    MeetingService
                ],
  entryComponents: [MeetingHomeComponent]
})
export class MeetingModule { }