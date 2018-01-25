import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import {ProgramindicatorsRoutingModule} from './programindicators-routing.module';

import {ProgramindicatorsListInitiativesComponent} from './programindicators-list-initiatives.component';
import {ProgramindicatorsListProgramsComponent} from './programindicators-list-programs.component';
import {ProgramindicatorsHomeComponent} from './programindicators-home.component';
import {ProgramindicatorsInitiativeDetailComponent} from './programindicators-initiative-detail.component';
import {ProgramindicatorsListIndicatorsComponent} from './programindicators-list-indicators.component';
import {ProgramindicatorsFormComponent} from './programindicators-form.component';
import { ProgramindicatorsIndicatorDetailComponent } from './programindicators-indicator-detail.component';



@NgModule({
  imports:      [   SharedModule, 
                    ProgramindicatorsRoutingModule
                        ],
  declarations: [   
                    ProgramindicatorsListInitiativesComponent,
                    ProgramindicatorsHomeComponent,
                    ProgramindicatorsListProgramsComponent,
                    ProgramindicatorsInitiativeDetailComponent,
                    ProgramindicatorsListIndicatorsComponent,
                    ProgramindicatorsFormComponent,
                    ProgramindicatorsIndicatorDetailComponent
  ],

})
export class ProgramindicatorsModule { }