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
import { AssignmentAffirmativeReportComponent } from './assignments/assignment-affirmative-report.component';
import { AssignmentAffirmativePlanComponent } from './assignments/assignment-affirmative-plan.component';
import { AssignmentProgramIndicatorsComponent } from './assignments/assignment-program-indicators.component';
import { AffirmativeModule } from '../affirmative/affirmative.module';
import { DistrictEmployeesComponent } from './district-employees.component';
import { DistrictEmployeeBriefComponent } from './district-employee-brief.component';



@NgModule({
  imports:      [   SharedModule,
                    DistrictRoutingModule,
                    PlansofworkModule,
                    AffirmativeModule
                    ],
  declarations: [   DistrictHomeComponent,
                    CountyListComponent,
                    DistrictPlansComponent,
                    NotCountiesListComponent,
                    AssignmentPlansOfWorkComponent,
                    AssignmentAffirmativeReportComponent,
                    AssignmentAffirmativePlanComponent,
                    AssignmentProgramIndicatorsComponent,
                    DistrictEmployeesComponent,
                    DistrictEmployeeBriefComponent
                    ],
  providers:    [     
                    DistrictService,
                    CountyService
                ],
  exports: [
                    AssignmentPlansOfWorkComponent,
                    AssignmentAffirmativeReportComponent,
                    AssignmentAffirmativePlanComponent,
                    AssignmentProgramIndicatorsComponent,
                    CountyListComponent
  ]
})
export class DistrictModule { }