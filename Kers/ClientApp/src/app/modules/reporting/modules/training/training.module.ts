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
import { TrainingConvertComponent } from './training-convert.component';
import { TrainingConvertItemComponent } from './training-convert-item.component';
import { TrainingTranscriptComponent } from './training-transcript.component';
import { TrainingUpcommingComponent } from './training-upcomming.component';
import { TrainingPostAttendanceComponent } from './training-post-attendance.component';
import { TrainingPostAttendanceDetailComponent } from './training-post-attendance-detail.component';



@NgModule({
  declarations: [
    TrainingHomeComponent,
    TrainingFormComponent,
    TrainingCatalogComponent,
    TrainingDetailComponent,
    TrainingInfoComponent,
    TrainingConvertComponent,
    TrainingConvertItemComponent,
    TrainingTranscriptComponent,
    TrainingUpcommingComponent,
    TrainingPostAttendanceComponent,
    TrainingPostAttendanceDetailComponent
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
