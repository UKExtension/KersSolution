import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LadderComponent } from './ladder.component';

const routes: Routes = [{
  path: '',
  component: LadderComponent,
  children: 
      []
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LadderRoutingModule { }
