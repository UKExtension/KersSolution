import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { HomeComponent } from './components/home/home.component';

import {ReportingModule} from './modules/reporting/reporting.module';
import {AuthenticationModule} from './modules/authentication/authentication.module';

import {CoreModule} from './modules/reporting/core/core.module'


@NgModule({
    declarations: [
        AppComponent,
        HomeComponent
    ],
    imports: [
        CommonModule,
        HttpModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'reporting', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: '**', redirectTo: 'reporting' }
        ]),
        ReportingModule,
        CoreModule,
        AuthenticationModule
    ]
})
export class AppModuleShared {
}
