import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {UserHomeComponent} from './user-home.component';

import {UserReportingHomeComponent} from './reporting/user-reporting-home.component';
import {UserPersonalHomeComponent} from './personal/user-personal-home.component';
import {UserComponent} from './user.component';
import {UserDirectoryComponent} from './directory/user-directory.component';
import { UserSummaryComponent } from "./summary/user-summary.component";

@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          component: UserHomeComponent,
          children: 
            [
                {
                  path: 'reporting',
                  component: UserReportingHomeComponent
                },
                {
                  path: 'personal',
                  component: UserPersonalHomeComponent
                },
                {
                  path: 'directory',
                  component: UserDirectoryComponent
                },
                {
                    path: ':id',
                    component: UserComponent,
                },
                {
                    path: 'summary/:id',
                    component: UserSummaryComponent,
                } 
              ] 
      }
             
  ])],
  exports: [ RouterModule ]
})
export class UserRoutingModule {}
