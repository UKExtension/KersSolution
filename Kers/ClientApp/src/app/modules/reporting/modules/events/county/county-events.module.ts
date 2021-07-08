import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { CountyEventsRoutingModule } from './county-events-routing.module';
import { CountyEventsHomeComponent } from './county-events-home.component';
import { LocationModule } from '../location/location.module';
import { CountyEventFormComponent } from './county-event-form.component';
import { AngularMyDatePickerModule } from 'angular-mydatepicker';
import { CountyEventListDetailsComponent } from './county-event-list-details.component';
import { CountyEventConvertItemComponent } from './county-events-convert-item.component';
import { CountyEventConvertComponent } from './county-events-convert.component';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';




@NgModule({
  imports:      [   
                  SharedModule,
                  AngularMyDatePickerModule,
                  CountyEventsRoutingModule,
                  LocationModule,
                  NgSelectModule,
                  FormsModule
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