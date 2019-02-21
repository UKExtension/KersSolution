import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { TrainingRoutingModule } from './training-routing.module';
import { TrainingHomeComponent } from './training-home.component';
import { TrainingFormComponent } from './training-form.component';
import { MyDatePickerModule } from 'mydatepicker';

@NgModule({
  declarations: [
    TrainingHomeComponent,
    TrainingFormComponent
  ],
  imports: [
    SharedModule,
    MyDatePickerModule,
    CommonModule,
    TrainingRoutingModule
  ]
})
export class TrainingModule { }
