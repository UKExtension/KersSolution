import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DemosRoutingModule } from './demos-routing.module';
import { DemoComponent } from './demo.component';
import { StoryDemoComponent } from './story-demo.component';
import { IndicatorsDemoComponent } from './indicators-demo.component';
import { StoryModule } from '../story/story.module';
import { SharedModule } from '../../shared/shared.module';
import { IndicatorsModule } from '../indicators/indicators.module';
import { PlansofworkFormDemoComponent } from './plansofwork-form-demo.component';
import { ProgramsService } from '../admin/programs/programs.service';
import { PlansofworkService } from '../plansofwork/plansofwork.service';
import { NgSelectModule } from '@ng-select/ng-select';


@NgModule({
  declarations: [
    DemoComponent,
    StoryDemoComponent,
    IndicatorsDemoComponent,
    PlansofworkFormDemoComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    DemosRoutingModule,
    StoryModule,
    IndicatorsModule,
    NgSelectModule
  ],
  providers: [
    PlansofworkService,
    ProgramsService
  ],
  entryComponents: [
    DemoComponent
  ]
})
export class DemosModule { }
