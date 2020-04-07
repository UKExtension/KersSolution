import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {ActivityHomeComponent} from './activity-home.component';
import {ActivityComponent} from './activity.component';
import {ActivityReportsHomeComponent} from './reports/activity-reports-home.component';
import {ActivityStatsHomeComponent} from './stats/activity-stats-home.component';
import {ActivityStatsAllComponent} from './stats/activity-stats-all.component';
import {ActivityStatsProgramComponent} from './stats/activity-stats-program.component';
import {ActivityStatsMonthComponent} from './stats/activity-stats-month.component';
import { ActivityFilterComponent } from './reports/filter/activity-filter.component';

@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          component: ActivityComponent,
          children: 
            [
                
                {
                  path: '',
                  component: ActivityHomeComponent
                },
                {
                  path: 'reports',
                  component: ActivityReportsHomeComponent
                },
                {
                  path: 'filter',
                  component: ActivityFilterComponent
                },
                {
                  path: 'stats',
                  component: ActivityStatsHomeComponent,
                  children: [
                    
                        {
                          path: '',
                          component: ActivityStatsAllComponent
                        },
                        {
                          path: 'month',
                          component: ActivityStatsMonthComponent
                        },
                        {
                          path: 'program',
                          component: ActivityStatsProgramComponent
                        },
                    
                  ]
                }
            ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class ActivityRoutingModule {}
