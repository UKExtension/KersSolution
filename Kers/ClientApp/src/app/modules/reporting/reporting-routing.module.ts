import { NgModule }             from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';

import {ReportingComponent} from './components/reporting/reporting.component';
import {ReportingListComponent} from './components/reporting/reporting-list.component';
import {ReportingDetailComponent} from './components/reporting/reporting-detail.component';
import {ReportingHomeComponent} from './components/reporting/reporting-home.component';
import {ReportingWidgetsComponent} from './components/reporting/reporting-widgets.component';


import {ProfileListComponent} from './components/reporting-profile/profile-list.component';
import {ReportingProfileEditComponent} from './components/reporting-profile/reporting-profile-edit.component';

import {AuthenticationGuard} from '../authentication/authentication-guard.service';
import { ReportingHelpHomeComponent } from './components/reporting-help/reporting-help-home.component';

const reportingRoutes: Routes = [
  {
    path: 'reporting',
    component: ReportingComponent,
    canActivate: [AuthenticationGuard],
    children: [
      {
        path: '',
        component: ReportingHomeComponent,
        canActivateChild: [AuthenticationGuard],
        children: [
          {
            path: 'admin', 
            loadChildren: './modules/admin/admin.module#AdminModule'},
          {
            path: 'plansofwork', 
            loadChildren: './modules/plansofwork/plansofwork.module#PlansofworkModule'
          },
          {
            path: 'affirmative', 
            loadChildren: './modules/affirmative/affirmative.module#AffirmativeModule'
          },
          {
            path: 'indicators', 
            loadChildren: './modules/indicators/indicators.module#IndicatorsModule'
          },
          {
            path: 'expense', 
            loadChildren: './modules/expense/expense.module#ExpenseModule'
          },
          {
            path: 'state', 
            loadChildren: './modules/state/state.module#StateModule'
          },
          {
            path: 'county', 
            loadChildren: './modules/county/county.module#CountyModule'
          },
          {
            path: 'planningunit', 
            loadChildren: './modules/planningunit/planningunit.module#PlanningunitModule'
          },
          {
            path: 'calendar', 
            loadChildren: './modules/calendar/kers-calendar.module#KersCalendarModule'
          },
          {
            path: 'activity', 
            loadChildren: './modules/activity/activity.module#ActivityModule'
          },
          {
            path: 'servicelog', 
            loadChildren: './modules/servicelog/servicelog.module#ServicelogModule'
          },
          {
            path: 'contact', 
            loadChildren: './modules/contact/contact.module#ContactModule'
          },
          {
            path: 'snaped', 
            loadChildren: './modules/snap-ed/snap-ed.module#SnapEdModule'
          },
          {
            path: 'story', 
            loadChildren: './modules/story/story.module#StoryModule'
          },
          {
            path: 'user', 
            loadChildren: './modules/user/user.module#UserModule'
          },
          {
            path: 'training', 
            loadChildren: './modules/training/training.module#TrainingModule'
          },
          {
            path: 'meeting', 
            loadChildren: './modules/meeting/meeting.module#MeetingModule'
          },
          {
            path: 'countyevents', 
            loadChildren: './modules/events/county/county-events.module#CountyEventsModule'
          },
          {
            path: 'budget', 
            loadChildren: './modules/budget/budget.module#BudgetModule'
          },
          {
            path: 'soildata', 
            loadChildren: './modules/soildata/soildata.module#SoildataModule'
          },
          {
            path: 'activity/:id',
            component: ReportingDetailComponent,
          },
          {
            path: 'list',
            component: ReportingListComponent
          },
          {
            path: 'profile',
            children: [
              {
                path: '',
                component: ProfileListComponent
              },
              {
                path: 'edit',
                component: ReportingProfileEditComponent

              }
            ]
          },
          {
            path: 'help',
            children: [
              {
                path: '',
                component: ReportingHelpHomeComponent
              },
              {
                path: ':id',
                component: ReportingHelpHomeComponent

              }
            ]
          },
          {
            path: '',
            component: ReportingWidgetsComponent
          }
        ]
      }
    ]
  }
];

@NgModule({
  declarations: [  ],
  imports: [
    RouterModule.forChild(reportingRoutes),
    CommonModule
  ],
  exports: [
    RouterModule
  ]
})
export class ReportingRoutingModule { }