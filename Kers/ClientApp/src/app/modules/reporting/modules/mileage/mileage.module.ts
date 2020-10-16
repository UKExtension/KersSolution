import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { MileageRoutingModule } from './mileage-routing.module';
import { MyDatePickerModule } from 'mydatepicker';
import { MileageHomeComponent } from './mileage-home.component';
import { MileageFormComponent } from './mileage-form.component';
import { LocationModule } from '../events/location/location.module';
import { MileageSegmentFormElementComponent } from './mileage-segment-form-element.component';


@NgModule({
  declarations: [MileageHomeComponent, MileageFormComponent, MileageSegmentFormElementComponent],
  imports: [
    CommonModule,
    MyDatePickerModule,
    SharedModule,
    MileageRoutingModule, 
    LocationModule
  ]
})
export class MileageModule { }
