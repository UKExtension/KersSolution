import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LadderComponent } from './ladder.component';
import { LadderApplicantComponent } from './ladder-applicant.component';
import { LadderReviewComponent } from './ladder-review.component';
import { LadderFilterComponent } from './ladder-filter.component';

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
  },
  {
    path: 'review/:stageId',
    component: LadderReviewComponent
  },
  {
    path: 'filter',
    component: LadderFilterComponent
  }
  
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LadderRoutingModule { }
