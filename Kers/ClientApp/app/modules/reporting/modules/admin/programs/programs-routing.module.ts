import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {ProgramsHomeComponent} from './programs-home.component';
import {InitiativeListComponent} from './initiative-list.component';

@NgModule({
  imports: [RouterModule.forChild([
     {
          path: '',
          component: ProgramsHomeComponent,
          children: [
            {
            path: '',
            component: InitiativeListComponent
            }
          ]
      }
             
  ])],
  exports: [RouterModule]
})
export class ProgramsRoutingModule {}
