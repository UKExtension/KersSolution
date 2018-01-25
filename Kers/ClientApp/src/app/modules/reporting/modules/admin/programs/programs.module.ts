import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import {ProgramsHomeComponent} from './programs-home.component';
import {ProgramsRoutingModule} from './programs-routing.module';

import {InitiativeDetailComponent} from './initiative-detail.component';
import {InitiativeFormComponent} from './initiative-form.component';
import {InitiativeListComponent} from './initiative-list.component';

import {ProgramsListComponent} from './programs-list.component';
import {ProgramDetailComponent} from './program-detail.component';
import {ProgramFormComponent} from './program-form.component';


@NgModule({
  imports:      [ SharedModule, ProgramsRoutingModule ],
  declarations: [ ProgramsHomeComponent,
                  InitiativeDetailComponent,
                  InitiativeFormComponent,
                  InitiativeListComponent,
                  ProgramsListComponent,
                  ProgramDetailComponent,
                  ProgramFormComponent ],

})
export class ProgramsModule { }