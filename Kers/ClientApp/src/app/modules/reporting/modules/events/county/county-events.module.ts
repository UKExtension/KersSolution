import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { CountyEventsRoutingModule } from './county-events-routing.module';
import { CountyEventsHomeComponent } from './county-events-home.component';
import { LocationModule } from '../location/location.module';





@NgModule({
  imports:      [   SharedModule,
                    CountyEventsRoutingModule,
                    LocationModule
                ],
  declarations: [ 
                    
                CountyEventsHomeComponent],
  providers:    [  
                    
                ],
  entryComponents: [CountyEventsHomeComponent]
})
export class CountyEventsModule { }