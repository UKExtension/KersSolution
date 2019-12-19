import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { LocationRoutingModule } from './location-routing.module';
import { LocationHomeComponent } from './location-home.component';
import { LocationFormComponent } from './location-form.component';





@NgModule({
  imports:      [   SharedModule,
                    LocationRoutingModule
                ],
  declarations: [ 
                    
                LocationHomeComponent,
                LocationFormComponent
              ],
  providers:    [  
                    
                ],
  exports: [ LocationHomeComponent ],
  entryComponents: [LocationHomeComponent]
})
export class LocationModule { }