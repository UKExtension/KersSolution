import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {UsersHomeComponent} from './users-home.component';
import {UsersListComponent} from './users-list.component';
//import {ReportingAdminRolesListComponent} from './reporting-admin-roles-list.component';

@NgModule({
  imports: [RouterModule.forChild([
      {
          path: '',
          component: UsersHomeComponent,
          children: [
            {
              path: '',
              component: UsersListComponent
            } 
          ]
      }           
  ])],
  exports: [RouterModule]
})
export class UsersRoutingModule {}
