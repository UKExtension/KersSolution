import { NgModule }            from '@angular/core';
import { CommonModule }        from '@angular/common';

import { FormsModule, ReactiveFormsModule }         from '@angular/forms';
//https://github.com/basvandenberg/ng-select
import { SelectModule } from 'ng-select';

import { DatepickerModule } from 'angular2-material-datepicker';
import { ReportingDisplayHelpComponent } from '../components/reporting-help/reporting-display-help.component';



import {LoadingComponent} from './components/loading.component';
import {TiimepickerComponent} from './components/timepicker.component';


import {echartsDirective} from './directives/echarts.directive';

import {SafeHtmlPipe} from "./pipes/safe-html-pipe.pipe";



//https://github.com/Gbuomprisco/ng2-tag-input
import { TagInputModule } from 'ngx-chips';

// Import the Froala Editor plugin.
import "froala-editor/js/froala_editor.pkgd.min.js";

// Import Angular plugin.
import { FroalaEditorModule, FroalaViewModule } from 'angular-froala-wysiwyg';


import { UserSocialPickerComponent } from '../modules/user/personal/user-social-picker.component';
import { UserPersonalImageComponent} from '../modules/user/personal/user-personal-image.component';
import { UserPersonalFormComponent } from '../modules/user/personal/user-personal-form.component';
import { UserPersonalConnectionComponent} from '../modules/user/personal/user-personal-connection.component';


import { UserReportingFormComponent } from '../modules/user/reporting/user-reporting-form.component';




@NgModule({
  imports:      [ 
                  CommonModule, 
                  TagInputModule,
                  FroalaEditorModule.forRoot(), 
                  FroalaViewModule.forRoot(),
                  FormsModule,
                  ReactiveFormsModule,
                  SelectModule
                  ],
                  
  declarations: [ 
                  LoadingComponent,
                  echartsDirective,
                  TiimepickerComponent,
                  ReportingDisplayHelpComponent,
                  UserPersonalFormComponent,
                  UserSocialPickerComponent,
                  UserPersonalImageComponent,
                  UserPersonalConnectionComponent,
                  UserReportingFormComponent,
                  SafeHtmlPipe 
                ],
  exports:      [ 
      CommonModule, 
      FormsModule, 
      ReactiveFormsModule,
      DatepickerModule,
      FroalaEditorModule,
      FroalaViewModule,
      LoadingComponent,
      echartsDirective,
      SafeHtmlPipe,
      TiimepickerComponent,
      ReportingDisplayHelpComponent,
      UserPersonalFormComponent,
      UserSocialPickerComponent,
      UserPersonalImageComponent,
      UserPersonalConnectionComponent,
      UserReportingFormComponent,
      SelectModule
 ]
})
export class SharedModule { }