import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule }   from '@angular/forms';
import { SharedModule } from '../reporting/shared/shared.module';

import {LoginComponent} from './components/login.component';
import {LoginHomeComponent} from './components/login-home.component';
import {LogoutComponent} from './components/logout.component';
import {AuthenticationService} from './authentication.service';
import {AuthenticationGuard } from './authentication-guard.service';
import {AuthHttp} from './auth.http';

@NgModule({
    declarations: [
        LoginComponent,
        LoginHomeComponent,
        LogoutComponent
    ],
    imports: [
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forChild([
            { path: 'login', component: LoginHomeComponent },
            { path: 'logout', component: LogoutComponent }
        ]),
        SharedModule
    ],
    exports: [
        RouterModule
    ],
    providers: [
        AuthenticationService,
        AuthenticationGuard,
        AuthHttp
    ]
})
export class AuthenticationModule {
}
