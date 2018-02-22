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
          //Commitment for next fiscal year of the current user
          path: 'commitment',
          component: CommitmentHomeComponent
        },
        {
          //Commitment for the current user of the specified fiscal year
          path: 'commitment/:fiscalyearid',
          component: CommitmentHomeComponent
        },
        {
          path: 'commitment/:fiscalyearid/:userid',
          component: CommitmentHomeComponent
        }
        
    ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SnapEdRoutingModule { }
