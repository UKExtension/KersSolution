import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SnapEdHomeComponent } from './snap-ed-home.component';
import { CommitmentHomeComponent } from './commitment/commitment-home.component';

const routes: Routes = [{
  path: '',
  children: 
    [
        
        {
          path: '',
          component: SnapEdHomeComponent
        },
        {
          path: 'commitment',
          component: CommitmentHomeComponent
        }
        
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SnapEdRoutingModule { }
