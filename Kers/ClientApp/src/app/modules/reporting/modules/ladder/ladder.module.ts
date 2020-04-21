import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { LadderRoutingModule } from './ladder-routing.module';
import { LadderComponent } from './ladder.component';
import { LadderApplicationFormComponent } from './ladder-application-form.component';

@NgModule({
  declarations: [LadderComponent, LadderApplicationFormComponent],
  imports: [
    CommonModule,
    SharedModule,
    LadderRoutingModule
  ]
})
export class LadderModule { }
