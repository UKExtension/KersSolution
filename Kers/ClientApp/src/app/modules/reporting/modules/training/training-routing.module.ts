import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TrainingHomeComponent } from './training-home.component';
import { TrainingCatalogComponent } from './training-catalog.component';
import { TrainingFormComponent } from './training-form.component';
import { TrainingInfoComponent } from './training-info.component';
import { TrainingConvertComponent } from './training-convert.component';
import { TrainingTranscriptComponent } from './training-transcript.component';
import { TrainingUpcommingComponent } from './training-upcomming.component';
import { TrainingPostAttendanceComponent } from './training-post-attendance.component';
import { TrainingProposalsAwaitingComponent } from './training-proposals-awaiting.component';
import { TrainingAdminCatalogComponent } from './training-admin-catalog.component';
import { TrainingAdminReportsComponent } from './training-admin-reports.component';
import { TrainingEnrollmentComponent } from './training-enrollment.component';
import { TrainingManagersComponent } from './training-managers.component';
import { RolesAuthGuard } from '../../shared/auth/roles-auth.guard';
import { TrainingFormSessionsComponent } from './training-form-sessions.component';

const routes: Routes = [{
  path: '',
  component: TrainingHomeComponent,
  children: 
    [
        {
          path: 'catalog',
          component: TrainingCatalogComponent
        },
        {
          path: 'propose',
          canActivate: [RolesAuthGuard],
          component: TrainingFormComponent
        },
        {
          path: 'convert',
          component: TrainingConvertComponent
        },
        {
          path: 'transcript',
          component: TrainingTranscriptComponent
        },
        {
          path: 'upcomming',
          component: TrainingUpcommingComponent
        },
        {
          path: 'postattendance',
          canActivate: [RolesAuthGuard],
          component: TrainingPostAttendanceComponent
        },
        {
          path: 'proposals',
          component: TrainingProposalsAwaitingComponent
        },
        {
          path: 'admincatalog',
          component: TrainingAdminCatalogComponent
        },
        {
          path: 'adminreports',
          component: TrainingAdminReportsComponent
        },
        {
          path: 'adminenrollment',
          component: TrainingEnrollmentComponent
        },
        {
          path: 'adminmanagers',
          component: TrainingManagersComponent
        },
        {
          path: ':id',
          component: TrainingInfoComponent
        }
        
        
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TrainingRoutingModule { }
