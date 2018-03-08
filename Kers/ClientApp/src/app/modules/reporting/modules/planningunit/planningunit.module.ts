import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { PlanningunitRoutingModule } from './planningunit-routing.module';
import { PlanningunitService } from './planningunit.service';
import { PlanningunitListComponent } from './planningunit-list.component';
import { PlanningUnitAdminHomeComponent } from './admin/planning-unit-admin-home.component';
import { PlanningUnitAdminFormComponent } from './admin/planning-unit-admin-form.component';
import { CountyDetailComponent } from './admin/county-detail.component';




@NgModule({
  imports:      [   SharedModule,
                    PlanningunitRoutingModule
                ],
  declarations: [ 
                    PlanningunitListComponent, PlanningUnitAdminHomeComponent, PlanningUnitAdminFormComponent, CountyDetailComponent

                ],
  providers:    [  
                    PlanningunitService
                ],
  exports:      [
                    PlanningunitListComponent
                ]
})
export class PlanningunitModule { }