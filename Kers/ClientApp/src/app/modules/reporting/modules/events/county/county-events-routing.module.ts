import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CountyEventsHomeComponent } from './county-events-home.component';


@NgModule({
  imports: [ RouterModule.forChild([
     
      {
        path: '',
        component: CountyEventsHomeComponent
        
      }
             
  ])],
  exports: [ RouterModule ]
})
export class CountyEventsRoutingModule {}
