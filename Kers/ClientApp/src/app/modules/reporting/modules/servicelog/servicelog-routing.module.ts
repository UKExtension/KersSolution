import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ServicelogComponent } from "./servicelog.component";
import { ServicelogHomeComponent } from "./servicelog-home.component";
import { ServicelogSnapedComponent } from "./snap-ed/servicelog-snaped.component";
import { ServicelogSnapedStatsComponent } from "./snap-ed/servicelog-snaped-stats.component";
import { ServicelogSnapedReportComponent } from './snap-ed/report/servicelog-snaped-report.component';

@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          component: ServicelogComponent,
          children: 
            [
                
                {
                  path: '',
                  component: ServicelogHomeComponent
                },
                {
                  path: 'snaped',
                  component: ServicelogSnapedComponent
                },
                {
                  path: 'snaped/report',
                  component: ServicelogSnapedReportComponent
                },
                {
                  path: ':fy',
                  component: ServicelogHomeComponent
                }
            ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class ServicelogRoutingModule {}
