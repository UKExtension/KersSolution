import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { SignupComponent } from './signup.component';
import { SignupFormComponent } from './signup-form.component';
import { SignupAttendiesComponent } from './signup-attendies.component';
import { SignupListRowComponent } from './signup-list-row.component';



@NgModule({
  declarations: [
    SignupComponent,
    SignupFormComponent,
    SignupAttendiesComponent,
    SignupListRowComponent
  ],
  imports: [
    SharedModule
  ],
  exports:      [
    SignupComponent,
    SignupAttendiesComponent
]
})
export class SignupModule { }
