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




@NgModule({
  imports:      [   SharedModule,
                    CountyRoutingModule,
                    PlansofworkModule,
                    AffirmativeModule,
                    UserModule
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