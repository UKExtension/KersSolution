import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';


import { IndicatorsHomeComponent } from './indicators-home.component';
import { RouterModule } from "@angular/router";
import { IndicatorsFormComponent } from './indicators-form.component';
import { IndicatorsDemoComponent } from './indicators-demo.component';

@NgModule({
  imports:      [ SharedModule,
                  RouterModule.forChild([
                      { path: '', component: IndicatorsHomeComponent },
                      {path: 'demo', component: IndicatorsDemoComponent}
                  ]),
        ],
  declarations: [ 
                  IndicatorsHomeComponent, IndicatorsFormComponent, IndicatorsDemoComponent
                ]
})
export class IndicatorsModule { }