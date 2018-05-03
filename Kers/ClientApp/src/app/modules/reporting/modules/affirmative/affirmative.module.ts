import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';

import {AffirmativeRoutingModule} from './affirmative-routing.module';
import {AffirmativeHomeComponent} from './affirmative-home.component';
import {AffirmativeFormComponent} from './affirmative-form.component';
import {AffirmativeReportsHomeComponent} from './reports/affirmative-reports-home.component';

import {AffirmativeService} from './affirmative.service';
import { AffirmativeHomeReportComponent } from './affirmative-home-report.component';

@NgModule({
  imports:      [ SharedModule, AffirmativeRoutingModule ],
  declarations: [ AffirmativeHomeComponent,
                  AffirmativeHomeReportComponent,
                  AffirmativeFormComponent,
                  AffirmativeReportsHomeComponent
                ],
  providers:    [
                  AffirmativeService
                ],
  exports:      [
                  AffirmativeReportsHomeComponent
                ]
})
export class AffirmativeModule { }