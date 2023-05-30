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
            loadChildren: () => import('./modules/admin/admin.module').then(m => m.AdminModule)
            
          },
          {
            path: 'plansofwork', 
            loadChildren: () => import('./modules/plansofwork/plansofwork.module').then(m => m.PlansofworkModule)
          },
          {
            path: 'affirmative', 
            loadChildren: () => import('./modules/affirmative/affirmative.module').then(m => m.AffirmativeModule)
          },
          {
            path: 'indicators', 
            loadChildren: () => import('./modules/indicators/indicators.module').then(m => m.IndicatorsModule)
          },
          {
            path: 'expense', 
            loadChildren: () => import('./modules/expense/expense.module').then(m => m.ExpenseModule)
          },
          {
            path: 'state', 
            loadChildren: () => import('./modules/state/state.module').then(m => m.StateModule)
          },
          {
            path: 'county', 
            loadChildren: () => import('./modules/county/county.module').then(m => m.CountyModule)
          },
          {
            path: 'planningunit', 
            loadChildren: () => import('./modules/planningunit/planningunit.module').then(m => m.PlanningunitModule)
          },
          {
            path: 'calendar', 
            loadChildren: () => import('./modules/calendar/kers-calendar.module').then(m => m.KersCalendarModule)
          },
          {
            path: 'activity', 
            loadChildren: () => import('./modules/activity/activity.module').then(m => m.ActivityModule)
          }, 
          {
            path: 'servicelog', 
            loadChildren: () => import('./modules/servicelog/servicelog.module').then(m => m.ServicelogModule)
            
          }, 
          {
            path: 'contact', 
            loadChildren: () => import('./modules/contact/contact.module').then(m => m.ContactModule)
          },
          {
            path: 'snaped', 
            loadChildren: () => import('./modules/snap-ed/snap-ed.module').then(m => m.SnapEdModule)
          },
          {
            path: 'story', 
            loadChildren:  () => import('./modules/story/story.module').then(m => m.StoryModule)
          },
          {
            path: 'user', 
            loadChildren: () => import('./modules/user/user.module').then(m => m.UserModule)
          },
          {
            path: 'training', 
            loadChildren: () => import('./modules/training/training.module').then(m => m.TrainingModule)
          },
          {
            path: 'meeting', 
            loadChildren: () => import('./modules/meeting/meeting.module').then(m => m.MeetingModule)
          },
          {
            path: 'countyevents', 
            loadChildren: () => import('./modules/events/county/county-events.module').then(m => m.CountyEventsModule)
          },
          {
            path: 'demos', 
            loadChildren: () => import('./modules/demos/demos.module').then(m => m.DemosModule)
          }, 
          {
            path:'ladder',
            loadChildren: () => import('./modules/ladder/ladder.module').then(m => m.LadderModule)
          },
          {
            path:'mileage',
            loadChildren: () => import('./modules/mileage/mileage.module').then(m => m.MileageModule)
          },
          {
            path:'extensionarea',
            loadChildren: () => import('./modules/area/area.module').then(m => m.AreaModule)
          },
          {
            path:'extensionregion',
            loadChildren: () => import('./modules/region/region.module').then(m => m.RegionModule)
          },
          {
            path: 'soildata', 
            loadChildren: () => import('./modules/soildata/soildata.module').then(m => m.SoildataModule)
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
            path: '',
            component: ReportingWidgetsComponent
          },
          { path: 'help', loadChildren: () => import('./modules/help/help.module').then(m => m.HelpModule) }
        ]
      }
    ]
  },
  
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