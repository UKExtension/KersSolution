import { NgModule } from '@angular/core';

import { SharedModule } from '../../shared/shared.module';
import {StoryRoutingModule} from './story-routing.module';

import {StoryComponent} from './story.component';
import {StoryHomeComponent} from './story-home.component';
import {StoryFormComponent} from './story-form.component';
import {StoryListComponent} from './story-list.component';
import {StoryDetailComponent} from './story-detail.component';
import {StoryService} from './story.service';
import {PlansofworkService} from '../plansofwork/plansofwork.service';
import {StoryReportsDetailComponent} from './reports/story-reports-detail.component';
import {StoryReportsFullComponent} from './reports/story-reports-full.component';
import { StoryReportsListComponent } from './reports/story-reports-list.component';
import { StoryReportsHomeComponent } from './reports/story-reports-home.component';
import { StoryReportsDisplayListComponent } from './reports/story-reports-display-list.component';
import { StoryDirectoryComponent } from './reports/story-directory.component';
import { StoryAuthorComponent } from './reports/story-author.component';
import { StoryDisplayComponent } from './reports/story-display.component';
import {StoryReportsDisplayListSyncComponent} from './reports/story-reports-display-list-sync.component';
import { StoryShortComponent } from './reports/story-short.component';
import { StoryFormDemoComponent } from './story-form-demo.component';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  imports:      [ SharedModule,
                  StoryRoutingModule,
                  NgSelectModule
                ],
  exports: [
    StoryFormDemoComponent
  ],
  declarations: [ 
                  StoryComponent,
                  StoryHomeComponent,
                  StoryFormComponent,
                  StoryFormDemoComponent,
                  StoryListComponent,
                  StoryDetailComponent,
                  StoryReportsDetailComponent,
                  StoryReportsFullComponent,
                  StoryReportsListComponent,
                  StoryReportsHomeComponent,
                  StoryReportsDisplayListComponent,
                  StoryReportsDisplayListSyncComponent,
                  StoryDirectoryComponent,
                  StoryAuthorComponent,
                  StoryDisplayComponent,
                  StoryShortComponent
                 
                ],
  providers:    [  
                  PlansofworkService,
                  StoryService
                ]
})
export class StoryModule { }