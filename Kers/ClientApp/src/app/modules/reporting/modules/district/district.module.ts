import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';

import {DistrictRoutingModule} from './district-routing.module';

import {DistrictHomeComponent} from './district-home.component';
import {CountyListComponent} from './county-list.component';
import {DistrictService} from './district.service';
import {DistrictPlansComponent} from './district-plans.component';

import {PlansofworkModule} from '../plansofwork/plansofwork.module';
import { CountyService } from '../county/county.service';
import { NotCountiesListComponent } from './not-counties-list.component';
import { AssignmentPlansOfWorkComponent } from './assignments/assignment-plans-of-work.component';
import { AssignmentAffirmativeeReportComponent } from './assignments/assignment-affirmativee-report.component';
import { AssignmentAffirmativeePlanComponent } from './assignments/assignment-affirmativee-plan.component';
import { AssignmentProgramIndicatorsComponent } from './assignments/assignment-program-indicators.component';



@NgModule({
  imports:      [   SharedModule,
                    DistrictRoutingModule,
                    PlansofworkModule
                    ],
  declarations: [   DistrictHomeComponent,
                    CountyListComponent,
                    DistrictPlansComponent,
                    NotCountiesListComponent,
                    AssignmentPlansOfWorkComponent,
                    AssignmentAffirmativeeReportComponent,
                    AssignmentAffirmativeePlanComponent,
                    AssignmentProgramIndicatorsComponent
                    ],
  providers:    [     
                    DistrictService,
                    CountyService
                ],
  exports: [
                    AssignmentPlansOfWorkComponent
  ]
})
export class DistrictModule { }