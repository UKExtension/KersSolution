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
import { Login2faComponent } from './components/login-2fa.component';

@NgModule({
    declarations: [
        LoginComponent,
        Login2faComponent,
        LoginHomeComponent,
        LogoutComponent
    ],
    imports: [
        FormsModule,
        ReactiveFormsModule,
        RouterModule.forChild([
            { path: 'login', component: LoginHomeComponent },
            { path: 'login2fa', component: Login2faComponent },
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
