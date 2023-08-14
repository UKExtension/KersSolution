import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DemosRoutingModule } from './demos-routing.module';
import { DemoComponent } from './demo.component';
import { StoryDemoComponent } from './story-demo.component';
import { IndicatorsDemoComponent } from './indicators-demo.component';
import { StoryModule } from '../story/story.module';
import { SharedModule } from '../../shared/shared.module';


@NgModule({
  declarations: [
    DemoComponent,
    StoryDemoComponent,
    IndicatorsDemoComponent
  ],
  imports: [
    SharedModule,
    CommonModule,
    DemosRoutingModule,
    StoryModule
  ],
  entryComponents: [
    DemoComponent
  ]
})
export class DemosModule { }
