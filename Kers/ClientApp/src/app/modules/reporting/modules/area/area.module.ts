import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { DistrictModule } from '../district/district.module';
import { AreaHomeComponent } from './area-home.component';
import { AreaRoutingModule } from './area-routing.module';





@NgModule({
  imports:      [   
                    CommonModule,
                    SharedModule,
                    AreaRoutingModule,
                    DistrictModule
                    ],
  declarations: [   
                    AreaHomeComponent],
  providers:    [     
                    
                ],
  exports: [
                    
  ]
})
export class AreaModule { }