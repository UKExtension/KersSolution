import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CountyEventsHomeComponent } from './county-events-home.component';
import { CountyEventConvertComponent } from './county-events-convert.component';


@NgModule({
  imports: [ RouterModule.forChild([
     
      {
        path: 'convert',
        component: CountyEventConvertComponent
      },
      {
        path: '',
        component: CountyEventsHomeComponent,
      }
             
  ])],
  exports: [ RouterModule ]
})
export class CountyEventsRoutingModule {}
