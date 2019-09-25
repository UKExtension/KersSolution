import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MeetingHomeComponent } from './meeting-home.component';
import { MeetingListComponent } from './meeting-list.component';


@NgModule({
  imports: [ RouterModule.forChild([
     
      {
        path: '',
        component: MeetingHomeComponent,
        children: [
          {
            path: 'admin',
            component: MeetingListComponent
          }
        ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class MeetingRoutingModule {}
