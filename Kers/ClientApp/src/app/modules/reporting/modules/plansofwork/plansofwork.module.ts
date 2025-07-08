import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';

import {PlansofworkRoutingModule} from './plansofwork-routing.module';
import {PlansofworkHomeComponent} from './plansofwork-home.component';

import {PlansofworkMapsComponent} from './plansofwork-maps.component';
import {PlansofworkMapFormComponent} from './plansofwork-map-form.component';
import {PlansofworkMapsDetailComponent} from './plansofwork-maps-detail.component';

import {PlansofworkComponent} from './plansofwork.component';
import {PlansofworkFormComponent} from './plansofwork-form.component';
import {PlansofworkDetailComponent} from './plansofwork-detail.component';
import {PlansofworkReportsComponent} from './plansofwork-reports.component';
import {PlansofworkViewComponent} from './plansofwork-view.component';

import {PlansofworkService} from './plansofwork.service';


import {ProgramsService} from '../admin/programs/programs.service';
import { NgSelectModule } from '@ng-select/ng-select';


@NgModule({
  imports:      [ SharedModule, PlansofworkRoutingModule, NgSelectModule ],
  declarations: [ PlansofworkHomeComponent, 
                  PlansofworkMapsComponent,
                  PlansofworkMapFormComponent,
                  PlansofworkMapsDetailComponent,
                  PlansofworkComponent,
                  PlansofworkFormComponent,
                  PlansofworkDetailComponent,
                  PlansofworkReportsComponent,
                  PlansofworkViewComponent ],
  providers:    [ PlansofworkService, 
                  ProgramsService],
  exports:      [
                  
                  PlansofworkDetailComponent,
                  PlansofworkReportsComponent,
                  PlansofworkViewComponent
                ]
})
export class PlansofworkModule { }