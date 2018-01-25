import { Component, OnInit, Input, EventEmitter, Output, SimpleChanges } from '@angular/core';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';
import { ProfileService, Profile } from '../../../components/reporting-profile/profile.service';
import { UsersService, KersUser, PersonalProfile} from './users.service';

@Component({
    selector: 'personal-profile-edit',
    templateUrl: 'personal-profile-edit.component.html' 
})
export class PersonalProfileEditComponent implements OnInit{

    
    errorMessage: string;
    editForm = null;
    formLoaded = false;

    @Input ('user') user: KersUser = null;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private fb: FormBuilder,
        private router: Router,
        private usersService: UsersService
    ){

        this.editForm = fb.group(
            {
              
              firstName: [''],
              lastName: [''],
              officePhone: [''],
              mobilePhone: [''],
              bio: ['']

            })
        

    }

    ngOnChanges(changes: SimpleChanges) {
        if(!this.formLoaded){
            if(this.user != null){
                 this.editForm.patchValue(this.user.personalProfile);
                 this.formLoaded = true;
            }
        }
    }

   onSubmit(){            

      this.usersService.updatePersonalProfile(this.user.id, this.editForm.value).subscribe(
            response => {
                this.user.personalProfile = <PersonalProfile> response;
                this.editForm.patchValue(this.user.personalProfile);
                this.onFormSubmit.emit();
            }
        );
            
    }

    ngOnInit(){
  
    }

    OnCancel(){
        
        this.onFormCancel.emit();
        
    }   
}