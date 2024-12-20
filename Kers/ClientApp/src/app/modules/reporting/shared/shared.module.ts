import { NgModule }            from '@angular/core';
import { CommonModule }        from '@angular/common';

import { FormsModule, ReactiveFormsModule }         from '@angular/forms';
//https://github.com/ng-select/ng-select
import { NgSelectModule } from '@ng-select/ng-select';

import { ReportingDisplayHelpComponent } from '../components/reporting-help/reporting-display-help.component';



import {LoadingComponent} from './components/loading.component';
import {TiimepickerComponent} from './components/timepicker.component';


import {echartsDirective} from './directives/echarts.directive';

import {SafeHtmlPipe} from "./pipes/safe-html-pipe.pipe";



//https://github.com/Gbuomprisco/ng2-tag-input
import { TagInputModule } from '@vpetrusevici/ngx-chips';

// Import the Froala Editor plugin.
//import "froala-editor/js/froala_editor.pkgd.min.js";

// Import Angular plugin.
import { FroalaEditorModule, FroalaViewModule } from 'angular-froala-wysiwyg';


import { UserSocialPickerComponent } from '../modules/user/personal/user-social-picker.component';
import { UserPersonalFormComponent } from '../modules/user/personal/user-personal-form.component';
import { UserPersonalConnectionComponent} from '../modules/user/personal/user-personal-connection.component';


import { UserReportingFormComponent } from '../modules/user/reporting/user-reporting-form.component';
import { FiscalYearSwitcherComponent } from './components/fiscal-year-switcher.component';
import { ImageUploadComponent } from './form-controls/image-upload.component';
import { MonthSwitcherComponent } from './components/month-switcher.component';
import {CopyClipboardDirective} from './directives/copy-clipboard.directive';
import { TimeFormat } from './pipes/convertTimeFrom24To12Hours';
import { AlertWidgetComponent } from './components/alert-widget.component';
import { AlertBannerComponent } from './components/alert-banner.component';
import { ProgressBarComponent } from './components/progress-bar.component';




@NgModule({
  imports:      [ 
                  CommonModule, 
                  TagInputModule,
                  FroalaEditorModule.forRoot(), 
                  FroalaViewModule.forRoot(),
                  ReactiveFormsModule,
                  TagInputModule,
                  NgSelectModule,
                  FormsModule
                  ],
                  
  declarations: [ 
                  LoadingComponent,
                  ProgressBarComponent,
                  echartsDirective,
                  TiimepickerComponent,
                  ReportingDisplayHelpComponent,
                  UserPersonalFormComponent,
                  UserSocialPickerComponent,
                  UserPersonalConnectionComponent,
                  UserReportingFormComponent,
                  SafeHtmlPipe,
                  FiscalYearSwitcherComponent,
                  MonthSwitcherComponent,
                  ImageUploadComponent ,
                  CopyClipboardDirective,
                  TimeFormat,
                  AlertWidgetComponent,
                  AlertBannerComponent
                ],
  exports:      [ 
      CommonModule, 
      FormsModule,
      ReactiveFormsModule,
      FroalaEditorModule,
      FroalaViewModule,
      LoadingComponent,
      ProgressBarComponent,
      echartsDirective,
      SafeHtmlPipe,
      TiimepickerComponent,
      ReportingDisplayHelpComponent,
      UserPersonalFormComponent,
      UserSocialPickerComponent,
      UserPersonalConnectionComponent,
      UserReportingFormComponent,
      //SelectModule,
      FiscalYearSwitcherComponent,
      MonthSwitcherComponent,
      ImageUploadComponent,
      CopyClipboardDirective,
      TimeFormat,
      AlertWidgetComponent
 ]
})
export class SharedModule { }