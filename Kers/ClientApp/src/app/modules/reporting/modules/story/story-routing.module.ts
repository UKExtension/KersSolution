import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import {StoryHomeComponent} from './story-home.component';
import {StoryComponent} from './story.component';
import {StoryReportsDetailComponent} from './reports/story-reports-detail.component';
import { StoryReportsHomeComponent } from './reports/story-reports-home.component';
import { StoryDirectoryComponent } from './reports/story-directory.component';

@NgModule({
  imports: [ RouterModule.forChild([
     {
          path: '',
          component: StoryComponent,
          children: 
            [
                
                {
                  path: '',
                  component: StoryHomeComponent
                },
                {
                  path: 'reports',
                  component: StoryReportsHomeComponent
                },
                {
                  path: 'directory',
                  component: StoryDirectoryComponent
                },
                {
                  path: ':id',
                  component: StoryReportsDetailComponent
                }
                
            ]
      }
             
  ])],
  exports: [ RouterModule ]
})
export class StoryRoutingModule {}
