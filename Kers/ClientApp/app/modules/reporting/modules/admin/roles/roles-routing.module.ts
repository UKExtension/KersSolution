import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {ReportingAdminRolesListComponent} from './reporting-admin-roles-list.component';

@NgModule({
  imports: [RouterModule.forChild([
     {
          path: 'list',
          component: ReportingAdminRolesListComponent
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
export class RolesRoutingModule {}
