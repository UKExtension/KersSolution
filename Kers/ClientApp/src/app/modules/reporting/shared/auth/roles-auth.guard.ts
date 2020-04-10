import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { UserService } from '../../modules/user/user.service';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class RolesAuthGuard implements CanActivate, CanActivateChild {

  permissions:RouteRolesPermission[] = [
    <RouteRolesPermission>{
      url:"/reporting/training/propose",
      roles: ["SRVCTRNR"]
    },
    <RouteRolesPermission>{
      url:"/reporting/training/postattendance",
      roles: ["SRVCTRNR"]
    },
    <RouteRolesPermission>{
      url:"/reporting/admin/navigation",
      roles: ["SYSADM"]
    },
    <RouteRolesPermission>{
      url:"/reporting/admin/roles/list",
      roles: ["SYSADM"]
    },
    <RouteRolesPermission>{
      url:"/reporting/admin/log",
      roles: ["SYSADM", "LOGSVIEW"]
    },
    <RouteRolesPermission>{
      url:"/reporting/admin/fiscalyear",
      roles: ["SYSADM", "CESADM"]
    },
    <RouteRolesPermission>{
      url:"/reporting/admin/email/templates",
      roles: ["SYSADM", "CESADM", "SRVCADM"]
    },
    <RouteRolesPermission>{
      url:"/reporting/admin/users",
      roles: ["SYSADM", "CESADM", "DEXT", "UPRFLS"]
    },

  ];


  constructor(
    private userService: UserService, 
    private router: Router) {

  }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      var prms = this.permissions.filter( f => f.url == state.url);
      if(prms.length > 0){
        return this.checkRole(prms[0].roles);
      }
      return Observable.of(false);
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
          if( !res ){
            this.router.navigate(['/reporting']);
          }
        }
      )
    );
  }
  
}


class RouteRolesPermission{
  url:string;
  roles: string[];
}
