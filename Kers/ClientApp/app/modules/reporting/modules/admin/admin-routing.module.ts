import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {ReportingAdminHomeComponent} from './reportin-admin-home.component';

@NgModule({
  imports: [RouterModule.forChild([
     {
          path: 'admin-home',
          component: ReportingAdminHomeComponent
     },
     {
          path: 'roles', 
          loadChildren: './roles/roles.module#RolesModule'      
      },
      {
          path: 'users', 
          loadChildren: './users/users.module#UsersModule'      
      },
      {
          path: 'navigation', 
          loadChildren: './navigation/navigation.module#NavigationModule'      
      },
      {
          path: 'fiscalyear', 
          loadChildren: './fiscalyear/fiscalyear.module#FiscalyearModule'      
      },
      {
          path: 'snaped',
          loadChildren: './snaped/snaped.module#SnapedModule'
      },
      {
          path: 'programs', 
          loadChildren: './programs/programs.module#ProgramsModule'      
      },
      {
          path: 'indicators', 
          loadChildren: './programindicators/programindicators.module#ProgramindicatorsModule'      
      },
      {
          path: 'help', 
          loadChildren: './help/help.module#HelpModule'      
      },
      {
          path: 'log', 
          loadChildren: './log/log.module#LogModule'      
      }
      ,
      {
          path: 'email', 
          loadChildren: './email/email.module#EmailModule'      
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
export class AdminRoutingModule {}