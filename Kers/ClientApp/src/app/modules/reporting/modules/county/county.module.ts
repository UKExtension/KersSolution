import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import {CountyRoutingModule} from './county-routing.module';

import {CountyHomeComponent} from './county-home.component';
import {CountyService} from './county.service';

import {UserModule} from '../user/user.module';
import { PlansofworkModule } from "../plansofwork/plansofwork.module";
import { AffirmativeModule } from "../affirmative/affirmative.module";
import { UnitHomeComponent } from './unit-home.component';
import { KsuHomeComponent } from './ksu-home.component';
import { DistrictModule } from '../district/district.module';
import { ExpenseModule } from '../expense/expense.module';




@NgModule({
  imports:      [   SharedModule,
                    CountyRoutingModule,
                    PlansofworkModule,
                    AffirmativeModule,
                    UserModule,
                    DistrictModule,
                    ExpenseModule
                    ],
  declarations: [ CountyHomeComponent,
                  UnitHomeComponent,
                  KsuHomeComponent  
                    ],
  providers:    [     
                  CountyService
                ]
})
export class CountyModule { }