import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EmailHomeComponent } from './email-home.component';
import { EmailTemplateComponent } from './email-template.component';
import { RolesAuthGuard } from '../../../shared/auth/roles-auth.guard';



@NgModule({
  imports: [RouterModule.forChild([
     {
          path: '',
          component: EmailHomeComponent,
      },
      {
        path: 'templates',
        canActivate: [RolesAuthGuard],
        component: EmailTemplateComponent
      }
             
  ])],
  exports: [RouterModule]
})
export class EmailRoutingModule {}
