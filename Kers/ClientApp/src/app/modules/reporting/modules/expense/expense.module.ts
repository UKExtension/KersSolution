import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { MyDatePickerModule } from 'mydatepicker';

import {ExpenseHomeComponent} from './expense-home.component';
import {ExpenseFormComponent} from './expense-form.component';
import {ExpenseListComponent} from './expense-list.component';
import {ExpenseMonthComponent} from './expense-month.component';
import {ExpenseDetailComponent} from './expense-detail.component';
import {ExpenseComponent} from './expense.component';
import {ExpenseReportsHomeComponent} from './reports/expense-reports-home.component';
import {ExpenseReportsYearComponent} from './reports/expense-reports-year.component';
import {ExpenseReportsMonthComponent} from './reports/expense-reports-month.component';
import {ExpenseReportsDetailsComponent} from './reports/expense-reports-details.component';
import {ExpenseReportsSummaryComponent } from './reports/expense-reports-summary.component';


import { RouterModule } from "@angular/router";
import {ExpenseService} from './expense.service';
import { VehicleComponent } from './vehicle/vehicle.component';
import { VehicleCountyComponent } from './vehicle/vehicle-county.component';
import { VehicleFormComponent } from './vehicle/vehicle-form.component';
import { VehicleService } from './vehicle/vehicle.service';
import { PlanningunitModule } from '../planningunit/planningunit.module';
import { VehicleDistrictComponent } from './vehicle/vehicle-district.component';
import { VehicleListDetailComponent } from './vehicle/vehicle-list-detail.component';

@NgModule({
  imports:      [ SharedModule,
                  MyDatePickerModule,
                  PlanningunitModule,
                  RouterModule.forChild([
                      {   path: '', 
                          component: ExpenseComponent,
                          children: [
                                {
                                    path: '',
                                    component: ExpenseHomeComponent,
                                },  
                                {
                                    path: 'reports',
                                    component: ExpenseReportsHomeComponent,
                                },  
                                {
                                    path: 'vehicle',
                                    component: VehicleComponent,
                                },  
                                {
                                    path: 'vehicle/county/:id',
                                    component: VehicleCountyComponent,
                                },  
                                {
                                    path: 'vehicle/district/:id',
                                    component: VehicleDistrictComponent,
                                },
                                {
                                    path: 'bytype/:type',
                                    component: ExpenseHomeComponent,
                                } 
                              ]

                      
                     }
                  ]),
        ],
  declarations: [ 
                  ExpenseHomeComponent,
                  ExpenseFormComponent,
                  ExpenseListComponent,
                  ExpenseMonthComponent,
                  ExpenseDetailComponent,
                  ExpenseComponent,
                  ExpenseReportsHomeComponent,
                  ExpenseReportsYearComponent,
                  ExpenseReportsMonthComponent,
                  ExpenseReportsDetailsComponent,
                  ExpenseReportsSummaryComponent,
                  VehicleComponent,
                  VehicleCountyComponent,
                  VehicleDistrictComponent,
                  VehicleFormComponent,
                  VehicleListDetailComponent 
                ],
  providers:    [ 
                    ExpenseService,
                    VehicleService
                ],
  exports:      [
                  ExpenseReportsHomeComponent,
                  ExpenseFormComponent
                ]
})
export class ExpenseModule { }