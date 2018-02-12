import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DistrictHomeComponent } from './district-home.component';
import {CountyListComponent} from './county-list.component';
import {DistrictPlansComponent} from './district-plans.component';
import { NotCountiesListComponent } from './not-counties-list.component';

@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          children: 
            [
              {
                      path: 'plans',
                      component: DistrictPlansComponent
                },
                {
                  path: 'notcounties',
                  component: NotCountiesListComponent,
                },
                {
                  path: ':id',
                  component: DistrictHomeComponent,
                },
                {
                  path: '',
                  component: DistrictHomeComponent
                }
  
            ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class DistrictRoutingModule {}
