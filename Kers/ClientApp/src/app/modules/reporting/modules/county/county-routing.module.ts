import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {CountyHomeComponent} from './county-home.component';


@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          children: 
            [
                {
                  path: ':id',
                  component: CountyHomeComponent,
                },
                {
                  path: 'manager/dashboard',
                  component: CountyHomeComponent
                }
            ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class CountyRoutingModule {}
