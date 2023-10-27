import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExemptComponent } from './exempt.component';

const routes: Routes = [{ path: '', component: ExemptComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ExemptRoutingModule { }
