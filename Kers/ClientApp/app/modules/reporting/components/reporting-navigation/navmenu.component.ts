import { Component, OnInit } from '@angular/core';
import { NavigationService, NavSection, NavGroup, NavItem} from './navigation.service';
import {ProfileService, Profile} from '../reporting-profile/profile.service';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html'
})
export class NavMenuComponent implements OnInit {

    public navigation;
    profile:Profile;
    errorMessage: string;

    constructor( 
        private navService: NavigationService, 
        private profileService : ProfileService) 
    {
        //navService.getNavigation().then( navigation => this.navigation = navigation );



    }
 
    ngOnInit(){
        this.navService.nav().subscribe(
            res => {
                    
                    this.navigation =  res;
                },
            error =>  this.errorMessage = <any>error
        );
        this.profileService.currentUser().subscribe(
            profile => this.profile = profile,
            error => this.errorMessage = <any> error
        );
        
    }
}
