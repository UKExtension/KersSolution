import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { MeetingHomeComponent } from './meeting-home.component';
import { MeetingRoutingModule } from './meeting-routing.module';
import { MeetingService } from './meeting.service';




@NgModule({
  imports:      [   SharedModule,
                    MeetingRoutingModule
                ],
  declarations: [ 
                    MeetingHomeComponent

                ],
  providers:    [  
                    MeetingService
                ],
  entryComponents: [MeetingHomeComponent]
})
export class MeetingModule { }