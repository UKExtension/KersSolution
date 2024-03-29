import { Injectable }       from '@angular/core';
import {
  CanActivate, Router,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  CanActivateChild
}                           from '@angular/router';
import { AuthenticationService }      from './authentication.service';

@Injectable()
export class AuthenticationGuard implements CanActivate, CanActivateChild {
  constructor(
    private authService: AuthenticationService, 
    private router: Router
    ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    let url: string = state.url;
    return this.checkLogin(url);
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return this.canActivate(route, state);
  }

  checkLogin(url: string): boolean {
    if (this.authService.isLoggedIn()) { 
        return true; 
    }
    // Store the attempted URL for redirecting
    this.authService.redirectUrl = url;
    // Navigate to the login page with extras
    this.router.navigate(['/login2fa']);
    return false;
  }
}
