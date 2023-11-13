
import { SharedModule } from '../../shared/shared.module';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ExemptRoutingModule } from './exempt-routing.module';
import { ExemptComponent } from './exempt.component';
import { ExemptFormComponent } from './exempt-form.component';
import { ExemptListDetailComponent } from './exempt-list-detail.component';
import { ExemptListComponent } from './exempt-list.component';


@NgModule({
  declarations: [
    ExemptComponent,
    ExemptFormComponent,
    ExemptListDetailComponent,
    ExemptListComponent
  ],
  imports: [
    CommonModule,
    ExemptRoutingModule,
    SharedModule,
    NgSelectModule,
    FormsModule
  ]
})
export class ExemptModule { }
