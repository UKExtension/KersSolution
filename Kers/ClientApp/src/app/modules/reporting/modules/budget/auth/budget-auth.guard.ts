import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserService } from '../../user/user.service';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BudgetAuthGuard implements CanActivate, CanActivateChild {


  constructor(
    private userService: UserService, 
    private router: Router) {

  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this.checkRole(["FOPRTNS", "CESADM"]);
  }
  canActivateChild(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this.canActivate(next, state);
  }

  checkRole(roles:string[]):Observable<boolean>{
    return this.userService.currentUserHasAnyOfTheRoles(roles).pipe(
      tap(
        res => {
          if( ! res ){
            this.router.navigate(['/reporting']);
          }
        }
      )
    );
  }
  
}
