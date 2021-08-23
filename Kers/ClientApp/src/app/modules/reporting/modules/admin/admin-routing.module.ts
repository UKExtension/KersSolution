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
          loadChildren: () => import('./programs/programs.module').then(m => m.ProgramsModule)   
      },
      {
          path: 'indicators', 
          loadChildren: () => import('./programindicators/programindicators.module').then(m => m.ProgramindicatorsModule)     
      },
      {
          path: 'help',
          loadChildren: () => import('./help/help.module').then(m => m.HelpModule)    
      },
      {
          path: 'log', 
          canActivate: [RolesAuthGuard],
          loadChildren: () => import('./log/log.module').then(m => m.LogModule)   
      }
      ,
      {
          path: 'email',
          loadChildren: () => import('./email/email.module').then(m => m.EmailModule)
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