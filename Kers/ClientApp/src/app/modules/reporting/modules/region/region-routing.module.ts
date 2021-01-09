import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RegionHomeComponent } from './region-home.component';

const routes: Routes = [
  {
    path: ':id',
    component: RegionHomeComponent
  },
  {
    path: '',
    component: RegionHomeComponent
  }
  
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RegionRoutingModule {}
