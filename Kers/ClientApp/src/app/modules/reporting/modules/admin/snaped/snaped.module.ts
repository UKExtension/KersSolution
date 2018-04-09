import { NgModule } from '@angular/core';


import { MyDatePickerModule } from 'mydatepicker';
import { MyDateRangePickerModule } from 'mydaterangepicker';
import { SharedModule } from "../../../shared/shared.module";
import { ServicelogService } from "../../servicelog/servicelog.service";
import { SnapedService } from "../../servicelog/snaped.service";
import { SnapedRoutingModule } from './snaped-routing.module';
import { SnapedHomeComponent } from './snaped-home.component';
import { SnapedComponent } from './snaped.component';
import { SnapedAdminService } from './snaped-admin.service';
import { FiscalyearService } from '../fiscalyear/fiscalyear.service';
import { PlanningunitModule } from '../../planningunit/planningunit.module';
import { SnapedCountyComponent } from './snaped-county.component';
import { UserModule } from '../../user/user.module';
import { SnapedAssistantsListComponent } from './snaped-assistants-list.component';
import { SnapedUserComponent } from './snaped-user.component';
import { ServicelogModule } from '../../servicelog/servicelog.module';
import { SnapedReimbursmentFormComponent } from './snaped-reimbursment-form';
import { SnapedReimbursmentItem } from './snaped-reimbursment-item';
import { SnapedBudgetHomeComponent } from './snaped-budget-home.component';
import { SnapEdModule } from '../../snap-ed/snap-ed.module';




@NgModule({
  imports:      [ SharedModule,
                  MyDatePickerModule,
                  MyDateRangePickerModule,
                  SnapedRoutingModule,
                  PlanningunitModule,
                  UserModule,
                  ServicelogModule,
                  SnapEdModule
                ],
  declarations: [ 
                  SnapedHomeComponent,
                  SnapedComponent,
                  SnapedCountyComponent,
                  SnapedAssistantsListComponent,
                  SnapedUserComponent,
                  SnapedReimbursmentFormComponent,
                  SnapedReimbursmentItem,
                  SnapedBudgetHomeComponent

                ],
  providers:    [  
                    ServicelogService,
                    SnapedService,
                    SnapedAdminService,
                    FiscalyearService
                ],
  exports:      [
                    
                ]
})
export class SnapedModule { }