import {
  ModuleWithProviders, NgModule,
  Optional, SkipSelf }       from '@angular/core';
import { CommonModule }      from '@angular/common';
import { Title }  from '@angular/platform-browser';

import { UserService }       from '../modules/user/user.service';
import { GoogleAnalyticsEventsService } from './google-analytics-events.service';
import {ReportingHelpService} from '../components/reporting-help/reporting-help.service';
import {ProgramsService} from '../modules/admin/programs/programs.service';
import {IndicatorsService} from '../modules/indicators/indicators.service';
import { ReportingService } from "../components/reporting/reporting.service";
import { FiscalyearService } from '../modules/admin/fiscalyear/fiscalyear.service';

@NgModule({
  imports:      [ CommonModule ],
  declarations: [  ],
  exports:      [  ],
  providers:    [ 
                    GoogleAnalyticsEventsService,
                    ReportingHelpService,
                    UserService,
                    Title,
                    ProgramsService,
                    IndicatorsService,
                    ReportingService,
                    FiscalyearService,
                    ]
})
export class CoreModule {
}