import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HaySampleFormComponent } from './sample/hay-sample-form.component';
import { HayHomeComponent } from './hay-home.component';


const routes: Routes = [{
  path: '',
  component: HayHomeComponent,
  children: 
    [
       {
         path: 'sample',
         component: HaySampleFormComponent
       }
        
        
        
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HayRoutingModule { }
