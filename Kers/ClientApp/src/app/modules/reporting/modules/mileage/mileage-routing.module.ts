import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MileageHomeComponent } from './mileage-home.component';
const routes: Routes = [{
  path: '',
  component: MileageHomeComponent,
  children: 
      [

      ]
  }
  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MileageRoutingModule { }
