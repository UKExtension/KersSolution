import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {ReportingAdminHomeComponent} from './reportin-admin-home.component';
import { RolesAuthGuard } from '../../shared/auth/roles-auth.guard';

@NgModule({
  imports: [RouterModule.forChild([
     {
          path: 'admin-home',
          component: ReportingAdminHomeComponent
     },
     {
          path: 'roles', 
          canActivate: [RolesAuthGuard],
          loadChildren: './roles/roles.module#RolesModule'      
      },
      {
          path: 'users', 
          canActivate: [RolesAuthGuard],
          loadChildren: () => import('./users/users.module').then(m => m.UsersModule)    
      },
      {
          path: 'navigation', 
          canActivate: [RolesAuthGuard],
          loadChildren: './navigation/navigation.module#NavigationModule'      
      },
      {
          path: 'fiscalyear', 
          canActivate: [RolesAuthGuard],
          loadChildren: './fiscalyear/fiscalyear.module#FiscalyearModule'      
      },
      {
          path: 'snaped',
          loadChildren: () => import('./snaped/snaped.module').then(m => m.SnapedModule)
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
          canActivate: [RolesAuthGuard],
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