import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { HelpService, HelpCategory } from './help.service';
import {Location} from '@angular/common';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';
import { RolesService, Role } from '../roles/roles.service';
import { UsersService, Position } from '../users/users.service';


@Component({
    selector: 'help-category-form',
    templateUrl: 'help-category-form.component.html' 
})
export class HelpCategoryFormComponent implements OnInit{

    helpForm = null;
    @Input() help:HelpCategory = null;
    @Input() parentId:number = null;
    errorMessage: string;
    roles: Role[];
    positions: Position[];

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<HelpCategory>();

    constructor( 
        private service: HelpService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location,
        private rolesService: RolesService,
        private usersService: UsersService 
    ){

        

        this.helpForm = fb.group(
            {
              title: ['', Validators.required],
              description: [''],
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

    }

    onSubmit(){   
                
        if(this.help){
            
            this.service.updateHelpCategory(this.help.id, this.helpForm.value).
            subscribe(
                res => {
                    this.help = <HelpCategory> res;
                    this.onFormSubmit.emit(this.help);
                }
            );
        }else{
            this.service.addHelpCategory(this.helpForm.value, this.parentId).
            subscribe(
                res => {
                    this.onFormSubmit.emit(res);
                }
            );
        }
        
    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}