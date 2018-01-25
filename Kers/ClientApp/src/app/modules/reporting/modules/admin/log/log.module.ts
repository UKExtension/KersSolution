import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';

import { MyDateRangePickerModule } from 'mydaterangepicker';


import {LogRoutingModule} from './log-routing.module';
import {LogHomeComponent} from './log-home.component';
import {LogDetailComponent} from './log-detail.component';
import {LogService} from './log.service';

@NgModule({
  imports:      [ SharedModule, LogRoutingModule, MyDateRangePickerModule ],
  declarations: [ LogHomeComponent, LogDetailComponent ],
  providers: [LogService]
})
export class LogModule { }