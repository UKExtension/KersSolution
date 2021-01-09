import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import { DistrictModule } from '../district/district.module';
import { RegionHomeComponent } from './region-home.component';
import { RegionRoutingModule } from './region-routing.module';





@NgModule({
  imports:      [   
                    CommonModule,
                    SharedModule,
                    RegionRoutingModule,
                    DistrictModule
                    ],
  declarations: [   
    RegionHomeComponent],
  providers:    [     
                    
                ],
  exports: [
                    
  ]
})
export class RegionModule { }