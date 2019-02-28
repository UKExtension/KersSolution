import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { MyDatePickerModule } from 'mydatepicker';
import { MyDateRangePickerModule } from 'mydaterangepicker';
import {ServicelogRoutingModule} from './servicelog-routing.module';
import { ServicelogService } from "./servicelog.service";
import { ServicelogComponent } from "./servicelog.component";
import { ServicelogHomeComponent } from "./servicelog-home.component";
import { ServocelogListComponent } from "./servicelog-list.component";
import { ServicelogMonthComponent } from "./servicelog-month.component";
import { ServicelogDetailComponent } from "./servicelog-detail.component";
import { ServicelogFormComponent } from "./servicelog-form.component";
import { SnapClassicService } from "../activity/snap-classic.service";
import { SnapIndirectMethodsComponent } from "./form-component/snap-indirect-methods.component";
import { SnapIndirectReachedComponent } from "./form-component/snap-indirect-reached.component";
import { SnapDirectAudienceComponent } from "./form-component/snap-direct-audience.component";
import { SnapPolicyAimedComponent } from "./form-component/snap-policy-aimed.component";
import { SnapPolicyPartnersComponent } from "./form-component/snap-policy-partners.component";
import { ServicelogSnapedComponent } from "./snap-ed/servicelog-snaped.component";
import { ServicelogSnapedStatsComponent } from "./snap-ed/servicelog-snaped-stats.component";
import { SnapedService } from "./snaped.service";
import { SnapedCommitmentStatsComponent } from './snap-ed/snaped-commitment-stats.component';
import { ServicelogSnapedReportComponent } from './snap-ed/report/servicelog-snaped-report.component';
import { FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';
import { SnapedAdminService } from '../admin/snaped/snaped-admin.service';
import { ServicelogSnapedStatsRowComponent } from './snap-ed/servicelog-snaped-stats-row.component';
import { ActivityModule } from '../activity/activity.module';
import { SnapEdModule } from '../snap-ed/snap-ed.module';



@NgModule({
  imports:      [ SharedModule,
                  MyDatePickerModule,
                  MyDateRangePickerModule,
                  ServicelogRoutingModule,
                  ActivityModule,
                  SnapEdModule
                ],
  declarations: [ 
                    ServicelogHomeComponent,
                    ServicelogComponent,
                    ServocelogListComponent,
                    ServicelogMonthComponent,
                    ServicelogDetailComponent,
                    ServicelogFormComponent,
                    SnapIndirectMethodsComponent,
                    SnapIndirectReachedComponent,
                    SnapDirectAudienceComponent,
                    SnapPolicyAimedComponent,
                    SnapPolicyPartnersComponent,
                    ServicelogSnapedComponent,
                    ServicelogSnapedStatsComponent,
                    SnapedCommitmentStatsComponent,
                    ServicelogSnapedReportComponent,
                    ServicelogSnapedStatsRowComponent

                ],
  providers:    [  
                    ServicelogService,
                    SnapedService,
                    FiscalyearService,
                    SnapedAdminService
                ],
  exports:      [
                    ServicelogFormComponent,
                    ServicelogSnapedStatsComponent,
                    SnapedCommitmentStatsComponent,
                ]
})
export class ServicelogModule { }