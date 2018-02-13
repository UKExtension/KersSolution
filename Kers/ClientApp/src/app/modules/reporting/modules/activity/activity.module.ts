import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { MyDatePickerModule } from 'mydatepicker';
import { MyDateRangePickerModule } from 'mydaterangepicker';
import {ActivityRoutingModule} from './activity-routing.module';

import {ActivityComponent} from './activity.component';
import {ActivityHomeComponent} from './activity-home.component';
import {ActivityListComponent} from './activity-list.component';
import {ActivityFormComponent} from './activity-form.component';
import {ActivityMonthComponent} from './activity-month.component';
import {ActivityDetailComponent} from './activity-detail.component';

import {ActivityReportsHomeComponent} from './reports/activity-reports-home.component';
import {ActivityReportsYearComponent} from './reports/activity-reports-year.component';
import {ActivityReportsMonthComponent} from './reports/activity-reports-month.component';
import {ActivityReportsDetailsComponent} from './reports/activity-reports-details.component';
import {ActivityReportsSummaryComponent } from './reports/activity-reports-summary.component';

import {ActivityStatsHomeComponent} from './stats/activity-stats-home.component';
import {ActivityStatsAllComponent} from './stats/activity-stats-all.component';
import {ActivityStatsProgramComponent} from './stats/activity-stats-program.component';
import {ActivityStatsMonthComponent} from './stats/activity-stats-month.component';
import {ActivityStatsRow} from './stats/activity-stats-row.component';
import {ActivityStatsMonthRow} from './stats/activity-stats-month-row.component';
import {ActivityStatsProgramhRow} from './stats/activity-stats-program-row.component';

import {ActivityService} from './activity.service';
import {SnapClassicService} from './snap-classic.service';
import { ServiceLogFullDetailsComponent } from './reports/fulldetails/service-log-full-details.component';
import { ServiceLogSummaryRowComponent } from './reports/fulldetails/service-log-summary-row.component';
import { ServiceLogSnapDirectDetailsComponent } from './reports/fulldetails/service-log-snap-direct-details.component';
import { ServiceLogSnapIndirectDetailsComponent } from './reports/fulldetails/service-log-snap-indirect-details.component';
import { ServicelogService } from '../servicelog/servicelog.service';
import { ServiceLogSnapPolicyDetailsComponent } from './reports/fulldetails/service-log-snap-policy-details.component';

@NgModule({
  imports:      [ SharedModule,
                  MyDatePickerModule,
                  MyDateRangePickerModule,
                  ActivityRoutingModule
                  
                ],
  declarations: [ 
                    ActivityComponent,
                    ActivityHomeComponent,
                    ActivityListComponent,
                    ActivityFormComponent,
                    ActivityMonthComponent,
                    ActivityDetailComponent,
                    ActivityReportsHomeComponent,
                    ActivityReportsYearComponent,
                    ActivityReportsMonthComponent,
                    ActivityReportsDetailsComponent,
                    ActivityReportsSummaryComponent,
                    ActivityStatsHomeComponent,
                    ActivityStatsAllComponent,
                    ActivityStatsProgramComponent,
                    ActivityStatsMonthComponent,
                    ActivityStatsRow,
                    ActivityStatsMonthRow,
                    ActivityStatsProgramhRow,
                    ServiceLogFullDetailsComponent,
                    ServiceLogSummaryRowComponent,
                    ServiceLogSnapDirectDetailsComponent,
                    ServiceLogSnapIndirectDetailsComponent,
                    ServiceLogSnapPolicyDetailsComponent
                ],
  providers:    [  
                    ActivityService,
                    SnapClassicService,
                    ServicelogService
                ],
  exports:      [
                    ActivityHomeComponent,
                    ActivityReportsHomeComponent,
                    ActivityStatsMonthComponent,
                    ActivityStatsProgramComponent,
                    ActivityReportsSummaryComponent
                ]
})
export class ActivityModule { }