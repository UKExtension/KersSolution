import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';

import {AdminRoutingModule} from './admin-routing.module';
import {ReportingAdminHomeComponent} from './reportin-admin-home.component';
import {RolesService} from './roles/roles.service';


@NgModule({
  imports:      [ SharedModule, AdminRoutingModule ],
  declarations: [ ReportingAdminHomeComponent ],
  providers:    [RolesService]
})
export class AdminModule { }