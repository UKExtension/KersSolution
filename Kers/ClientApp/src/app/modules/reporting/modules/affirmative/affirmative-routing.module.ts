import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {AffirmativeHomeComponent} from './affirmative-home.component';
import {AffirmativeReportsHomeComponent} from './reports/affirmative-reports-home.component';
import { AffirmativeHomeReportComponent } from './affirmative-home-report.component';


@NgModule({
  imports: [RouterModule.forChild([
      {
          path: '',
          component: AffirmativeHomeComponent,
      },
      {
          path: 'reports',
          component: AffirmativeReportsHomeComponent
      },
      {
          path: 'report',
          component: AffirmativeHomeReportComponent
      },
      {
        path: ':fy',
        component: AffirmativeHomeComponent,
      },
      {
          path: 'reports/:fy',
          component: AffirmativeReportsHomeComponent
      }         
  ])],
  exports: [RouterModule]
})
export class AffirmativeRoutingModule {}
