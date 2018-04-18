import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { KersCalendarComponent } from './kers-calendar.component';

const routes: Routes = [
  {
      path: '',          
      children: [
        {
          path: '',
          component: KersCalendarComponent
        } 
      ]

  }           
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CalendarRoutingModule { }
