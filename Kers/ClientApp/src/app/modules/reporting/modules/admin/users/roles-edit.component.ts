import { Component, Input, EventEmitter, Output, SimpleChanges } from '@angular/core';
import { ProfileService, Profile } from '../../../components/reporting-profile/profile.service';
import {UsersService, KersUser } from './users.service';
import {UserService, User} from '../../user/user.service';
import {RolesService, Role } from '../roles/roles.service';
import { FormBuilder, Validators }   from '@angular/forms';
import { Observable } from "rxjs";

@Component({
    selector: 'roles-edit-form',
    templateUrl: './roles-edit.component.html'
})
export class RolesEditComponent { 

    @Input ('user') user: Observable<User>;
    u:User;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    private roles:Role[];


    userRoleIds:number[];
    editFormRoles = null;

    errorMessage: string;

    constructor( 
        private usersService:UsersService,
        private rolesService:RolesService,
        private fb: FormBuilder,
    )   
    {}

    ngOnInit(){
        this.rolesService.listRoles().subscribe(
            roles =>  {
                this.roles = roles;
                this.user.subscribe(
                    usr =>  {
                        this.u = <User>usr
                        var group = {};
                        if(this.u.roles != null){
                            for(var i = 0; i < this.roles.length; i++){
                                var ifRole = this.u.roles.filter(l=>l.zEmpRoleTypeId == this.roles[i].id)[0];
                                if(ifRole != null){
                                    group[this.roles[i].id] = [ true ];
                                }else{
                                    group[this.roles[i].id] = [ false ];
                                }
                            }
                        }
                        this.editFormRoles = this.fb.group(
                                group )
                
                    },
                    error =>  this.errorMessage = <any>error
                );
            },
            error =>  this.errorMessage = <any>error
        );
        
    }
/*
    ngOnChanges(changes: SimpleChanges) {
        if(!this.userLoaded){
            if(this.user != null){
                this.usersService.roleIds(this.user).subscribe(
                    res =>{ 
                        this.userRoleIds = res;
                        this.loadUserRoles();
                    },
                    error =>  this.errorMessage = <any>error
                )
                this.userLoaded = true;
                  
            }
        }
    }

    loadUserRoles(){
        var group = {};
        for(var i = 0; i < this.roles.length; i++){
           group[this.roles[i].id] = [ this.userRoleIds.indexOf(this.roles[i].id) > -1  ];
        }
        this.editFormRoles = this.fb.group(
                 group )
    }

*/
    onSubmit(){
        var r : number[] = [];
        for(var i = 0; i < this.roles.length; i++){
            if( this.editFormRoles.value[this.roles[i].id] ){
                r.push(this.roles[i].id)
            }
        }
        
        this.usersService.updateRoles(this.u.id, r).subscribe(
            response => {
                //this.user.personalProfile = <PersonalProfile> response;
                //this.editForm.patchValue(this.user.personalProfile);
                //console.log(response);
                this.onFormSubmit.emit();
            }
        ); 
    }
    OnCancel(){
        
        this.onFormCancel.emit();
        
    } 

}