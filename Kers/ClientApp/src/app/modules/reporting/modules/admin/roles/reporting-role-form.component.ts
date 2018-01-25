import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { RolesService, Role } from './roles.service';
import {Location} from '@angular/common';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';

@Component({
    selector: 'reporting-role-form',
    templateUrl: 'reporting-role-form.component.html' 
})
export class ReportingRoleFormComponent implements OnInit{

    roleForm = null;
    @Input() role = null;
    errorMessage: string;
    public options: Object;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private rolesService: RolesService,
        private reportingService: ReportingService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location
    ){

        this.options = { 
            placeholderText: 'Your Description Here!',
            toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', '|', 'formatUL', 'formatOL','insertImage'],
            toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline'],
            toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline'],
            toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
            imageUploadURL: location.prepareExternalUrl('/FroalaApi/UploadImageResize'),
            fileUploadURL: location.prepareExternalUrl('/FroalaApi/UploadFile'),
            imageManagerLoadURL: location.prepareExternalUrl('/FroalaApi/LoadImages'),
            imageManagerDeleteURL: location.prepareExternalUrl('/FroalaApi/DeleteImage'),
            imageManagerDeleteMethod: "POST",
            events : { 
                'froalaEditor.contentChanged' : (e, editor) => { 
                        this.roleForm.patchValue({description: editor.html.get()}) 
                            } 
                        }
        }


/*
options: Object = { charCounterCount: false, placeholderText: 'Edit Your Content Here!', 
events : { 'froalaEditor.contentChanged' : (e, editor) => { this.myForm.patchValue({content: editor.html.get()}) } } }
*/

        this.roleForm = fb.group(
            {
              title: ['', Validators.required],
              shortTitle: ['', Validators.required],
              description: [''],
              enabled: [''],
              selfEnrolling: [''],
              created: [''],
              updated: ['']
            }
        );
    }
   
    ngOnInit(){
       if(this.role){
           this.roleForm.patchValue(this.role);
       }

    }

    onSubmit(){            
        if(this.role){
            this.rolesService.updateRole(this.role.id, this.roleForm.value).
            subscribe(
                res => {
                    this.role = <Role> res;
                    this.onFormSubmit.emit();
                    //console.log(res);
                }
            );
        }else{
            this.rolesService.addRole(this.roleForm.value).
            subscribe(
                res => {
                    this.onFormSubmit.emit();
                    //console.log(res.json());
                }
            );
        }
        
    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}