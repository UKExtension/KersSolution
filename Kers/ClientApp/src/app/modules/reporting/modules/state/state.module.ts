import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';

import {StateService} from './state.service';
import {StateRoutingModule} from './state-routing.module';
import {StateHomeComponent} from './state-home.component';
import {DistrictListComponent} from './district-list.component';
import { CountyService } from '../county/county.service';





@NgModule({
  imports:      [ SharedModule, 
                  StateRoutingModule,
                    ],
  declarations: [   StateHomeComponent,
                    DistrictListComponent
                    ],
  providers:    [     
                  StateService,
                  CountyService
                ]
})
export class StateModule { }