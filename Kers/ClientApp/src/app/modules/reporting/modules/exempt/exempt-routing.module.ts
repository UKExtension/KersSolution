import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExemptComponent } from './exempt.component';
import { ExemptReviewComponent } from './exempt-review.component';

const routes: Routes = [{ path: '', component: ExemptComponent },
          {path:'review', component: ExemptReviewComponent}

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExemptRoutingModule { }
