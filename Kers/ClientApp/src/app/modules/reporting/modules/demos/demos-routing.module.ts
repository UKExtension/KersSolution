import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DemoComponent } from './demo.component';
import { components } from 'knockout';
import { StoryDemoComponent } from './story-demo.component';
import { IndicatorsDemoComponent } from './indicators-demo.component';

const routes: Routes = [
  {
    path:'',
    component: DemoComponent,
    children:[{
      path: 'story',
      component: StoryDemoComponent
    },
    {
      path: 'indicators',
      component: IndicatorsDemoComponent
    }]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DemosRoutingModule { }
