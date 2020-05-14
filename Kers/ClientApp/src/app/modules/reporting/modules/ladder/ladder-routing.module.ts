import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LadderComponent } from './ladder.component';
import { LadderApplicantComponent } from './ladder-applicant.component';

const routes: Routes = [{
  path: '',
  component: LadderComponent,
  children: 
      [

      ]
  },
  {
    path: 'applicant',
    component: LadderApplicantComponent
  }
  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LadderRoutingModule { }
