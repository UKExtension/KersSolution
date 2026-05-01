import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '../../shared/shared.module';
import { AngularMyDatePickerModule } from 'angular-mydatepicker';

import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';
import { HayHomeComponent } from './hay-home.component';
import { HayRoutingModule } from './hay-routing.module';
import { HaySampleFormComponent } from './sample/hay-sample-form.component';
import { SoildataModule } from '../soildata/soildata.module';
import { HaySampleFormElementComponent } from './sample/hay-sample-form-element.component';



@NgModule({
  imports: [
    SharedModule,
    AngularMyDatePickerModule,
    CommonModule,
    NgSelectModule,
    FormsModule,
    HayRoutingModule,
    SoildataModule
  ],
  declarations: [
    HayHomeComponent,
    HaySampleFormComponent,
    HaySampleFormElementComponent
  ]
})
export class HayModule { }
