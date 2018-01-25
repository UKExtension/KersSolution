import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {ProgramindicatorsHomeComponent} from './programindicators-home.component'

@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          children: 
            [
                {
                  path: '',
                  component: ProgramindicatorsHomeComponent
                }
              ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class ProgramindicatorsRoutingModule {}
