import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';


import {RolesRoutingModule} from './roles-routing.module';
import {ReportingAdminRolesListComponent} from './reporting-admin-roles-list.component';
import {ReportingRoleFormComponent} from './reporting-role-form.component';
import {RolesListDetailComponent} from './roles-list-detail.component';

@NgModule({
  imports:      [ SharedModule, RolesRoutingModule ],
  declarations: [ ReportingAdminRolesListComponent, 
                  ReportingRoleFormComponent,
                  RolesListDetailComponent ]
})
export class RolesModule { }