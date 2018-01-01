import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {FiscalyearHomeComponent} from './fiscalyear-home.component';
import {FiscalyearListComponent} from './fiscalyear-list.component';

@NgModule({
  imports: [RouterModule.forChild([
     {
          path: '',
          component: FiscalyearHomeComponent,
          children: 
            [
                {
                  path: '',
                  component: FiscalyearListComponent
                }
              ]
      }
             
  ])],
  exports: [RouterModule]
})
export class FiscalyearRoutingModule {}
