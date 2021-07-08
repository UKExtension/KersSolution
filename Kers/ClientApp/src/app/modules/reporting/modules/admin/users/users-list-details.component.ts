import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { ProfileService, Profile } from '../../../components/reporting-profile/profile.service';
import { UsersService, KersUser, PersonalProfile } from './users.service';
import {UserService, User} from '../../user/user.service';
import { Observable } from 'rxjs';


@Component({
    selector: '[usersListDetail]',
    templateUrl: 'users-list-details.component.html'
})
export class UsersListDetailComponent implements OnInit {

    @Input ('usersListDetail') profile: Profile;

    @Output() onProfileUpdated = new EventEmitter();
    errorMessage: string;
    user:KersUser = null;


    u: Observable<User>

    editPersonal:boolean = false;
    editReporting:boolean = false;
    editRoles:boolean = false;
    row:boolean = true;

    constructor(
        private profileService: ProfileService,
        private usersService: UsersService,
        private uService: UserService
        ){
        
    }

    ngOnInit(){
        this.u = this.uService.user(this.profile.id);
    }
    
    loadUser(){
        /*
        if(this.user == null){
            this.usersService.byProfile(this.profile).subscribe(
                d => { 
                    this.user = d; 
            },
                error => this.errorMessage = <any> error
            );
        }
        */
    }


    onEditReporting(){
        this.row = false;
        this.editReporting = true;
        this.loadUser();
    }
    onEditPersonal(){
        this.row = false;
        this.editPersonal = true;
        this.loadUser();
    }
    onEditRoles(){
        this.row = false;
        this.editRoles = true;
        this.loadUser();
    }
    onClose(){
        this.row = true;
        this.editReporting = false;
        this.editPersonal = false;
        this.editRoles = false;
    }


    profileUpdated(){
        this.onProfileUpdated.emit();
        //console.log(this.profile)
        this.row = true;
        this.editReporting = false;
        this.editPersonal = false;
        this.editRoles = false;
    }
}
