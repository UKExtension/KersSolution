import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {CountyHomeComponent} from './county-home.component';
import { UnitHomeComponent } from './unit-home.component';
import { KsuHomeComponent } from './ksu-home.component';


@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          children: 
            [
                {
                  path: 'ksu',
                  component: KsuHomeComponent,
                },
                {
                  path: ':id',
                  component: CountyHomeComponent,
                },
                {
                  path: 'unit/:id',
                  component: UnitHomeComponent,
                },
                {
                  path: 'ksu',
                  component: KsuHomeComponent,
                },
                {
                  path: 'manager/dashboard',
                  component: CountyHomeComponent
                },
                {
                  path: ':id/:returnto',
                  component: UnitHomeComponent,
                }
            ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class CountyRoutingModule {}
