import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {NavigationHomeComponent} from './navigation-home.component';
import {NavigationSectionComponent} from './navigation-section.component';

@NgModule({
  imports: [RouterModule.forChild([
      {
          
          path: '',
          component: NavigationHomeComponent,
          children: [
            {
              path: '',
              component: NavigationSectionComponent
            } 
          ] 
      }          
  ])],
  exports: [RouterModule]
})
export class NavigationRoutingModule {}