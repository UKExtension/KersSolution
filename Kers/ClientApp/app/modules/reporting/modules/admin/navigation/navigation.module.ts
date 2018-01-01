import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';
import { RolesService } from '../roles/roles.service';
import { UsersService } from '../users/users.service';


import {NavigationRoutingModule} from './navigation-routing.module';

import {NavigationHomeComponent} from './navigation-home.component';
import {NavigationSectionComponent} from './navigation-section.component';
import {NavigationSectionDetailComponent} from './navigation-section-detail.component';
import {AdminNavigationService} from './admin-navigation.service';
import {NavigationSectionFormComponent} from './navigation-section-form.component';
import {NavigationGroupComponent} from './navigation-group.component';
import {NavigationGroupDetailComponent} from './navigation-group-detail.component';
import {NavigationGroupFormComponent} from './navigation-group-form.component';
import {NavigationItemsComponent} from './navigation-items.component';
import {NavigationItemDetailComponent} from './navigation-item-detail.component';
import {NavigationItemFormComponent} from './navigation-item-form.component';



@NgModule({
  imports:      [   SharedModule, 
                    NavigationRoutingModule
                ],
  declarations: [   
                    NavigationHomeComponent,
                    NavigationSectionComponent,
                    NavigationSectionDetailComponent,
                    NavigationSectionFormComponent,
                    NavigationGroupComponent,
                    NavigationGroupDetailComponent,
                    NavigationGroupFormComponent,
                    NavigationItemDetailComponent,
                    NavigationItemFormComponent,
                    NavigationItemsComponent
                ],
  providers:    [ 
                    AdminNavigationService,
                    RolesService,
                    UsersService
                 ]
})
export class NavigationModule { }