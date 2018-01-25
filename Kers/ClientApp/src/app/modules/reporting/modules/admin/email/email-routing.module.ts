import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EmailHomeComponent } from './email-home.component';



@NgModule({
  imports: [RouterModule.forChild([
     {
          path: '',
          component: EmailHomeComponent,
          /*
          children: 
            [
                {
                  path: '',
                  component: FiscalyearListComponent
                }
              ]
              */
      }
             
  ])],
  exports: [RouterModule]
})
export class EmailRoutingModule {}
