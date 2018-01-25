import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';


import { IndicatorsHomeComponent } from './indicators-home.component';
import { RouterModule } from "@angular/router";

@NgModule({
  imports:      [ SharedModule,
                  RouterModule.forChild([
                      { path: '', component: IndicatorsHomeComponent }
                  ]),
        ],
  declarations: [ 
                  IndicatorsHomeComponent
                ]
})
export class IndicatorsModule { }