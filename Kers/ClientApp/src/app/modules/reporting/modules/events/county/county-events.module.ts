import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { CountyEventsRoutingModule } from './county-events-routing.module';
import { CountyEventsHomeComponent } from './county-events-home.component';
import { LocationModule } from '../location/location.module';
import { CountyEventFormComponent } from './county-event-form.component';
import { MyDatePickerModule } from 'mydatepicker';
import { MyDateRangePickerModule } from 'mydaterangepicker';
import { CountyEventListDetailsComponent } from './county-event-list-details.component';
import { CountyEventConvertItemComponent } from './county-events-convert-item.component';
import { CountyEventConvertComponent } from './county-events-convert.component';




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
                  CountyEventFormComponent,
                  CountyEventListDetailsComponent,
                  CountyEventConvertComponent,
                  CountyEventConvertItemComponent
                ],
  providers:    [  
                    
                ],
  entryComponents: [CountyEventsHomeComponent]
})
export class CountyEventsModule { }