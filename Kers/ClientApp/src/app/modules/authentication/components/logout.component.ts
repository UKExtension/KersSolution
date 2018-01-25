import { Component, ViewEncapsulation } from '@angular/core';
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
        //this.deleteCookies();
        this.router.navigate(['/login']);
    }

    deleteCookies(){
        document.cookie = "COAgKersPersonID=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersInstID=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersPersonName=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersCntyID=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersCntyName=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersEmailDeliveryAddress=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersEmailDeliveryAddress=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersIsAgent=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersIsDD=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersPlanningUnitID=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersPlanningUnitName=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersPositionID=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersIsCesInServiceTrainer=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "COAgKersIsCesInServiceAdmin=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
    }
    
}