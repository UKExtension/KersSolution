import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MeetingHomeComponent } from './meeting-home.component';


@NgModule({
  imports: [ RouterModule.forChild([
     
      {
        path: '',
        component: MeetingHomeComponent
      }
             
  ])],
  exports: [ RouterModule ]
})
export class MeetingRoutingModule {}
