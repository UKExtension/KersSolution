import { NgModule, forwardRef } from '@angular/core';


import { SharedModule } from '../../shared/shared.module';

import {UserComponent} from './user.component';

import {UserHomeComponent} from './user-home.component';

import { UserReportingHomeComponent } from './reporting/user-reporting-home.component';

import { UserPersonalHomeComponent } from './personal/user-personal-home.component';

import {UserRoutingModule} from './user-routing.module';
import { UsersService } from '../admin/users/users.service';
import {UserDirectoryComponent} from './directory/user-directory.component';
import {UserDirectoryProfileComponent} from './directory/user-directory-profile.component';
import {UserDirectoryListComponent} from './directory/user-directory-list.component'
import {ActivityService} from '../activity/activity.service';
import {StoryService} from '../story/story.service';
import {UserSummaryComponent} from './summary/user-summary.component';

import {ActivityModule} from '../activity/activity.module';
import {ExpenseModule} from '../expense/expense.module';

@NgModule({
  imports:      [ SharedModule,
                  ActivityModule,
                  ExpenseModule,
                  UserRoutingModule ],
  declarations: [ 
                  UserHomeComponent,
                  UserReportingHomeComponent,
                  UserPersonalHomeComponent,
                  UserComponent,
                  UserDirectoryComponent,
                  UserDirectoryProfileComponent,
                  UserDirectoryListComponent,
                  UserSummaryComponent
                ],
  providers:    [     
                  UsersService,
                  ActivityService,
                  StoryService
                ],
  exports: [
                  UserDirectoryListComponent,
                  UserDirectoryProfileComponent 
  ]
})
export class UserModule { }