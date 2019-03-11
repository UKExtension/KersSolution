import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TrainingHomeComponent } from './training-home.component';
import { TrainingCatalogComponent } from './training-catalog.component';

const routes: Routes = [{
  path: '',
  component: TrainingHomeComponent,
  children: 
    [
        {
          path: 'catalog',
          component: TrainingCatalogComponent
        }
        
        
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TrainingRoutingModule { }
