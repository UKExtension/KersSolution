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
          loadChildren: () => import('./roles/roles.module').then(m => m.RolesModule)     
      },
      {
          path: 'users', 
          canActivate: [RolesAuthGuard],
          loadChildren: () => import('./users/users.module').then(m => m.UsersModule)    
      },
      {
          path: 'navigation', 
          canActivate: [RolesAuthGuard],
          loadChildren: () => import('./navigation/navigation.module').then(m => m.NavigationModule)  
      },
      {
          path: 'fiscalyear', 
          canActivate: [RolesAuthGuard],
          loadChildren: () => import('./fiscalyear/fiscalyear.module').then(m => m.FiscalyearModule)
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