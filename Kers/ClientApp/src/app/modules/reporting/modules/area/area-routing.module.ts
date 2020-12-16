import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AreaHomeComponent } from './area-home.component';

const routes: Routes = [{
  path: '',
  component: AreaHomeComponent
  }
  
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AreaRoutingModule {}
