import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import {Location} from '@angular/common';
import {UserService, User, PersonalProfile, PlanningUnit} from '../../modules/user/user.service';
import {AuthenticationService} from '../../../authentication/authentication.service';
import {Router} from "@angular/router";
import { PlanningunitService } from '../../modules/planningunit/planningunit.service';
import { Observable } from 'rxjs';

@Component({
    selector: 'top-nav-menu',
    templateUrl: './topnav.component.html',
    styles: [`
        .nav > li > a{
            padding: 5px 15px 5px;
        }
        .toggle{
            margin-top: 0px;
            padding-top: 8px;
        }
        .nav_menu{
            height:42px;
        }
    `]
})
export class TopNavComponent implements OnInit{

    @Output() onToggle = new EventEmitter<void>();

    public profilePicSrc;
    errorMessage: string;
    user:User = null;
    isOpen = false;
    isCountyChangeOpen = false;
    counties:Observable<PlanningUnit[]>;
    isShared:Observable<boolean>;

    constructor( 
                    private auth: AuthenticationService,
                    private userService: UserService,
                    private countyService: PlanningunitService,
                    public router: Router,
                    private location:Location
                     ){
        this.profilePicSrc = location.prepareExternalUrl('/assets/images/user.png');
                     }
    
    ngOnInit(){

/* 
        this.profileService.currentUser().subscribe(
            profile => this.profile = profile,
            error => this.errorMessage = <any> error
        )
 */
        this.counties = this.countyService.counties();
        this.isShared = this.userService.currentUserHasAnyOfTheRoles(['SHAGNT']);
        this.userService.current().subscribe(
            res=> {
                this.user = <User>res;
                if(this.user.personalProfile.uploadImage){
                    this.profilePicSrc = this.location.prepareExternalUrl('/image/crop/60/60/' + this.user.personalProfile.uploadImage.uploadFile.name);
                }
            },
            err => this.errorMessage = <any>err
        )
    }


    toggle_left(){
        this.onToggle.emit();    
    }

    logout(){
        this.auth.logout();
        this.router.navigate(["/"]);
    }

    changeCounty(id:number){
        this.isCountyChangeOpen = false;
        this.userService.changePlanningUnitTo(id).subscribe(
            res => {
                var newUser = <User>res;
                this.user.rprtngProfile = newUser.rprtngProfile;
            }

        )
    }
}