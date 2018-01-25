import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';

import {UsersRoutingModule} from './users-routing.module';
import {UsersHomeComponent} from './users-home.component';
import {UsersListComponent} from './users-list.component';
import {UsersListDetailComponent} from './users-list-details.component';
import {RolesEditComponent} from './roles-edit.component';
import {PersonalProfileEditComponent} from './personal-profile-edit.component';
import {UsersService} from './users.service';

import {ReportingModule} from '../../../reporting.module';

@NgModule({
  imports:      [ SharedModule, 
                  UsersRoutingModule, 
                  ReportingModule ],
  declarations: [ UsersHomeComponent, 
                  UsersListComponent, 
                  UsersListDetailComponent,
                  RolesEditComponent,
                  PersonalProfileEditComponent ],
  providers: [UsersService]
})
export class UsersModule { }