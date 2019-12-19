import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { CountyEventsRoutingModule } from './county-events-routing.module';
import { CountyEventsHomeComponent } from './county-events-home.component';
import { LocationModule } from '../location/location.module';
import { CountyEventFormComponent } from './county-event-form.component';
import { MyDatePickerModule } from 'mydatepicker';
import { MyDateRangePickerModule } from 'mydaterangepicker';




@NgModule({
  imports:      [   
                  SharedModule,
                  MyDatePickerModule,
                  MyDateRangePickerModule,
                  CountyEventsRoutingModule,
                  LocationModule
                ],
  declarations: [ 
                  CountyEventsHomeComponent,
                  CountyEventFormComponent
                ],
  providers:    [  
                    
                ],
  entryComponents: [CountyEventsHomeComponent]
})
export class CountyEventsModule { }