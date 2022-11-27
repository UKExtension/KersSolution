import { Component } from '@angular/core';
import { AuthenticationService} from '../authentication.service';
import { UserService } from '../../reporting/modules/user/user.service';
import { Router } from '@angular/router';


@Component({
  template: 'logout'
})
export class LogoutComponent { 
    
    


    constructor(
        private authService: AuthenticationService, 
        public router: Router,
        private userService:UserService
     )   
    {}

    ngOnInit(){
        this.authService.logout();
        this.userService.unset();
        this.router.navigate(['/login2fa']);
    }
}