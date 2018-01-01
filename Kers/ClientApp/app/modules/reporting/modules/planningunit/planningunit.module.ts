import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { PlanningunitRoutingModule } from './planningunit-routing.module';
import { PlanningunitService } from './planningunit.service';
import { PlanningunitListComponent } from './planningunit-list.component';




@NgModule({
  imports:      [   SharedModule,
                    PlanningunitRoutingModule
                ],
  declarations: [ 
                    PlanningunitListComponent

                ],
  providers:    [  
                    PlanningunitService
                ],
  exports:      [
                    PlanningunitListComponent
                ]
})
export class PlanningunitModule { }