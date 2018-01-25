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

@NgModule({
  imports:      [ SharedModule,
                  MyDatePickerModule,
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
                  ExpenseReportsSummaryComponent 
                ],
  providers:    [ ExpenseService ],
  exports:      [
                  ExpenseReportsHomeComponent,
                ]
})
export class ExpenseModule { }