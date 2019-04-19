import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { TrainingRoutingModule } from './training-routing.module';
import { TrainingHomeComponent } from './training-home.component';
import { TrainingFormComponent } from './training-form.component';
import { MyDatePickerModule } from 'mydatepicker';
import { TrainingCatalogComponent } from './training-catalog.component';
import { MyDateRangePickerModule } from 'mydaterangepicker';
import { TrainingDetailComponent } from './training-detail.component';
import { TrainingInfoComponent } from './training-info.component';



@NgModule({
  declarations: [
    TrainingHomeComponent,
    TrainingFormComponent,
    TrainingCatalogComponent,
    TrainingDetailComponent,
    TrainingInfoComponent
  ],
  imports: [
    SharedModule,
    MyDatePickerModule,
    MyDateRangePickerModule,
    CommonModule,
    TrainingRoutingModule
  ]
})
export class TrainingModule { }
