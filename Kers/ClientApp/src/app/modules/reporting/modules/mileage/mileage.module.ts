import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { MileageRoutingModule } from './mileage-routing.module';
import { MyDatePickerModule } from 'mydatepicker';
import { MileageHomeComponent } from './mileage-home.component';
import { MileageFormComponent } from './mileage-form.component';
import { LocationModule } from '../events/location/location.module';
import { MileageSegmentFormElementComponent } from './mileage-segment-form-element.component';
import { ExpenseModule } from '../expense/expense.module';
import { MileageMonthComponent } from './mileage-month.component';
import { MileageDetailComponent } from './mileage-detail.component';
import { ExpenseCompatabilityFormComponent } from './expense-compatability-form.component';


@NgModule({
  declarations: [ExpenseCompatabilityFormComponent, MileageHomeComponent, MileageFormComponent, MileageSegmentFormElementComponent, MileageMonthComponent, MileageDetailComponent],
  imports: [
    CommonModule,
    MyDatePickerModule,
    SharedModule,
    MileageRoutingModule, 
    LocationModule
  ],
  exports: [
    MileageFormComponent
  ]
})
export class MileageModule { }
