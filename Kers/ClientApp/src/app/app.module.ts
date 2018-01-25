import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import {HttpModule} from '@angular/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ReportingModule } from './modules/reporting/reporting.module';
import { CoreModule } from './modules/reporting/core/core.module';
import { AuthenticationModule } from './modules/authentication/authentication.module';


@NgModule({
  declarations: [
    AppComponent,
    HomeComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    BrowserAnimationsModule,
    HttpClientModule,
    HttpModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', redirectTo: 'reporting', pathMatch: 'full' },
      { path: 'home', component: HomeComponent },
      { path: '**', redirectTo: 'reporting' },
    ]),
    ReportingModule,
    CoreModule,
    AuthenticationModule
  ],
  providers: [HttpClientModule],
  bootstrap: [AppComponent]
})
export class AppModule { }
