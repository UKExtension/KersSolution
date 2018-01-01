import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { DatepickerModule } from 'angular2-material-datepicker';


import {FiscalyearRoutingModule} from './fiscalyear-routing.module';
import {FiscalyearHomeComponent} from './fiscalyear-home.component';
import {FiscalyearListComponent} from './fiscalyear-list.component';
import {FiscalyearDetailComponent} from './fiscalyear-detail.component';
import {FiscalyearFormComponent} from './fiscalyear-form.component';

import {FiscalyearService} from './fiscalyear.service';


@NgModule({
  imports:      [ SharedModule, 
                  FiscalyearRoutingModule, 
                  DatepickerModule ],
  declarations: [ FiscalyearHomeComponent, 
                  FiscalyearListComponent,
                  FiscalyearDetailComponent,
                  FiscalyearFormComponent ],
  providers:    [     
                  FiscalyearService 
                ]
})
export class FiscalyearModule { }