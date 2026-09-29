import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {StateHomeComponent} from './state-home.component';
import {DistrictListComponent} from './district-list.component';


@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          component: StateHomeComponent,
          children: 
            [
                {
                  path: '',
                  component: DistrictListComponent
                },
                {
                  path: 'district', 
                  loadChildren: () => import('../district/district.module').then(m => m.DistrictModule)
                }
              ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class StateRoutingModule {}
