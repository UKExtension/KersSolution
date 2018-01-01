import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {HelpHomeComponent} from './help-home.component';
import {HelpCategoryComponent} from './help-category.component';
import {HelpListComponent} from './help-list.component';

@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          component: HelpHomeComponent,
          children: 
            [
                {
                  path: '',
                  component: HelpListComponent
                },
                {
                  path: 'category',
                  component: HelpCategoryComponent
                }
              ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class HelpRoutingModule {}
