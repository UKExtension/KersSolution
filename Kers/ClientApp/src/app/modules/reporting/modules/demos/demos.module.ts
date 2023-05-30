import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DemosRoutingModule } from './demos-routing.module';
import { DemoComponent } from './demo.component';


@NgModule({
  declarations: [
    DemoComponent
  ],
  imports: [
    CommonModule,
    DemosRoutingModule
  ],
  entryComponents: [
    DemoComponent
  ]
})
export class DemosModule { }
