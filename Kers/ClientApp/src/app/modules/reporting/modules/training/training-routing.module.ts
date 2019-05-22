import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TrainingHomeComponent } from './training-home.component';
import { TrainingCatalogComponent } from './training-catalog.component';
import { TrainingFormComponent } from './training-form.component';
import { TrainingInfoComponent } from './training-info.component';
import { TrainingConvertComponent } from './training-convert.component';
import { TrainingTranscriptComponent } from './training-transcript.component';
import { TrainingUpcommingComponent } from './training-upcomming.component';

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
        },
        {
          path: 'convert',
          component: TrainingConvertComponent
        },
        {
          path: 'transcript',
          component: TrainingTranscriptComponent
        },
        {
          path: 'upcomming',
          component: TrainingUpcommingComponent
        },
        {
          path: ':id',
          component: TrainingInfoComponent
        }
        
        
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TrainingRoutingModule { }
