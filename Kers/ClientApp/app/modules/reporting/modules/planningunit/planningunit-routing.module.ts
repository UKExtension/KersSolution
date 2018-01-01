import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';


@NgModule({
  imports: [ RouterModule.forChild([
     {
         /*
          path: '',
          component: PlanningunitComponent,
          children: 
            [
                
                {
                  path: '',
                  component: ServicelogHomeComponent
                },
                {
                  path: 'snaped',
                  component: ServicelogSnapedComponent
                }
               
            ] */
      }
             
  ])],
  exports: [ RouterModule ]
})
export class PlanningunitRoutingModule {}
