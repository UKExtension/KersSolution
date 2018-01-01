import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {LogHomeComponent} from './log-home.component';

@NgModule({
  imports: [RouterModule.forChild([
     {
          path: '',
          component: LogHomeComponent
        }
              
              
              /*
              ,
              {
                path: 'users',
                component: ProfileListComponent
              },
              
              {
                path: 'roles',
                component: ReportingAdminRolesListComponent
              }
              */
             
  ])],
  exports: [RouterModule]
})
export class LogRoutingModule {}
