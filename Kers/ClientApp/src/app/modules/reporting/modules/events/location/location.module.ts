import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { LocationRoutingModule } from './location-routing.module';
import { LocationHomeComponent } from './location-home.component';
import { LocationFormComponent } from './location-form.component';
import { LocationDetailComponent } from './location-detail.component';





@NgModule({
  imports:      [   SharedModule,
                    LocationRoutingModule
                ],
  declarations: [ 
                    
                LocationHomeComponent,
                LocationFormComponent,
                LocationDetailComponent
              ],
  providers:    [  
                    
                ],
  exports: [ LocationHomeComponent ],
  entryComponents: [LocationHomeComponent]
})
export class LocationModule { }