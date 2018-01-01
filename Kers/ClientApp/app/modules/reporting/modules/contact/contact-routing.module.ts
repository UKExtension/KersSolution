import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {ContactHomeComponent} from './contact-home.component';
import {ContactComponent} from './contact.component';

import {ContactStatsHomeComponent} from './stats/contact-stats-home.component';
import {ContactStatsAllComponent} from './stats/contact-stats-all.component';
import {ContactStatsMonthComponent} from './stats/contact-stats-month.component';
import {ContactStatsProgramComponent} from './stats/contact-stats-program.component';

@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          component: ContactComponent,
          children: 
            [
                
                {
                  path: '',
                  component: ContactHomeComponent,
                },
                {
                  path: 'stats',
                  component: ContactStatsHomeComponent,
                  children: [
                      {
                        path: '',
                        component: ContactStatsAllComponent,
                      },
                      {
                        path: 'month',
                        component: ContactStatsMonthComponent
                      },
                      {
                        path: 'program',
                        component: ContactStatsProgramComponent
                      }
                  ]
                }
            ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class ContactRoutingModule {}
