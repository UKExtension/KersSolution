import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TrainingHomeComponent } from './training-home.component';
import { TrainingCatalogComponent } from './training-catalog.component';
import { TrainingFormComponent } from './training-form.component';

const routes: Routes = [{
  path: '',
  component: TrainingHomeComponent,
  children: 
    [
        {
          path: 'catalog',
          component: TrainingCatalogComponent
        },
        {
          path: 'propose',
          component: TrainingFormComponent
        }
        
        
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TrainingRoutingModule { }
