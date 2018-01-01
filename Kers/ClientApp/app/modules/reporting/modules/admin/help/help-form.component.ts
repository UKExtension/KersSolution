import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { HelpService, Help, HelpCategory } from './help.service';
import {Location} from '@angular/common';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';
import {ProfileService, Profile} from '../../../components/reporting-profile/profile.service';
import { RolesService, Role } from '../roles/roles.service';
import { UsersService, Position } from '../users/users.service';

@Component({
    selector: 'help-form',
    templateUrl: 'help-form.component.html' 
})
export class HelpFormComponent implements OnInit{

    helpForm = null;
    @Input() help:Help = null;

    categories:HelpCategory[];
    errorMessage: string;

    roles: Role[];
    positions: Position[];

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Help>();

    public options: Object;

    constructor( 
        private service: HelpService,
        private profileService: ProfileService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location,
        private rolesService: RolesService,
        private usersService: UsersService 
    ){

        this.options = { 
            placeholderText: 'Help Content Here!',
            toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL','insertImage', 'insertVideo', '|', 'insertTable', 'html'],
            toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat', '|','insertImage', 'html'],
            toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline',  'paragraphFormat', '|','insertImage'],
            toolbarButtonsXS: ['bold', 'italic',  'paragraphFormat', '|','insertImage'],
            videoInsertButtons: ['videoBack', '|', 'videoByURL', 'videoEmbed'],
            imageUploadParams: { profileId: this.profileService.profile.id },
            imageUploadURL: location.prepareExternalUrl('/FroalaApi/UploadImage'),
            fileUploadURL: location.prepareExternalUrl('/FroalaApi/UploadFile'),
            imageManagerLoadURL: location.prepareExternalUrl('/FroalaApi/LoadImages'),
            imageManagerDeleteURL: location.prepareExternalUrl('/FroalaApi/DeleteImage'),
            imageManagerDeleteMethod: "POST"
        }

        this.helpForm = fb.group(
            {
              title: ['', Validators.required],
              categoryId: ['', Validators.required],
              body: [''],
              employeePositionId: [''],
              zEmpRoleTypeId: [''],
              isContyStaff: 0,
            }
        );
    }
   
    ngOnInit(){

       if(this.help){
           this.helpForm.patchValue(this.help);
       }
       this.rolesService.listRoles().subscribe(
                res => {
                    this.roles = <Role[]>res;
                },
                error => this.errorMessage = <any> error
        );
        this.usersService.positions().subscribe(
                res => {
                    this.positions = <Position[]>res;
                },
                error => this.errorMessage = <any> error
        );
       this.service.allCategories().
            subscribe(
                res => {
                    this.categories = res;
                },
                error => this.errorMessage = <any>error
            );

    }

    onSubmit(){   



        if(this.help){
            
            this.service.updateHelp(this.help.id, this.helpForm.value).
            subscribe(
                res => {
                    this.help = <Help> res;
                    this.onFormSubmit.emit(this.help);
                }
            );
            
        }else{
            this.service.addHelp(this.helpForm.value).
            subscribe(
                res => {
                    this.onFormSubmit.emit(<Help>res);
                }
            );
        }
        
    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}