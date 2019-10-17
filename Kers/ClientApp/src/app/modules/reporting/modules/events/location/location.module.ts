import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { LocationRoutingModule } from './location-routing.module';
import { LocationHomeComponent } from './location-home.component';





@NgModule({
  imports:      [   SharedModule,
                    LocationRoutingModule
                ],
  declarations: [ 
                    
                LocationHomeComponent],
  providers:    [  
                    
                ],
  exports: [LocationHomeComponent],
  entryComponents: [LocationHomeComponent]
})
export class LocationModule { }