import { NgModule } from '@angular/core';

import { SharedModule } from '../../../shared/shared.module';

import {HelpService} from './help.service';
import {HelpRoutingModule} from './help-routing.module';

import {HelpCategoryComponent} from './help-category.component';
import {HelpDetailComponent} from './help-detail.component';
import {HelpFormComponent} from './help-form.component';
import {HelpListComponent} from './help-list.component';
import {HelpHomeComponent} from './help-home.component';
import {HelpCategoryListComponent} from './help-category-list.component';
import {HelpCategoryFormComponent} from './help-category-form.component';

import {HelpCategoryDetailComponent} from './help-category-detail.component';
import { RolesService } from '../roles/roles.service';
import { UsersService } from '../users/users.service';




@NgModule({
  imports:      [ SharedModule, 
                  HelpRoutingModule
                   ],
  declarations: [ HelpCategoryComponent,
                  HelpDetailComponent,
                  HelpFormComponent,
                  HelpListComponent,
                  HelpHomeComponent,
                  HelpCategoryDetailComponent,
                  HelpCategoryListComponent,
                  HelpCategoryFormComponent
   ],
  providers:    [     
                  HelpService,
                  RolesService,
                  UsersService
                ]
})
export class HelpModule { }