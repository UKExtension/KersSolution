import { NgModule } from '@angular/core';
import { AngularMyDatePickerDirective } from './angular-mydatepicker.directive';

@NgModule({
  declarations: [AngularMyDatePickerDirective],
  exports: [AngularMyDatePickerDirective]
})
export class AngularMyDatePickerModule { }
