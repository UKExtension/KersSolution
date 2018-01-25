import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import {Location} from '@angular/common';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';
import {AdminNavigationService} from './admin-navigation.service';
import {  NavSection, NavGroup } from '../../../components/reporting-navigation/navigation.service';
import {RolesService, Role} from '../roles/roles.service';
import { UsersService, Position } from '../users/users.service';


@Component({
    selector: 'navigation-group-form',
    templateUrl: 'navigation-group-form.component.html' 
})
export class NavigationGroupFormComponent implements OnInit{

    groupForm = null;
    @Input() group:NavGroup = null;
    @Input() section:NavSection = null;
    errorMessage: string;

    roles: Role[];
    positions: Position[];

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<NavGroup>();

    constructor( 
        private reportingService: ReportingService,
        private service: AdminNavigationService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location,
        private rolesService: RolesService,
        private usersService: UsersService 
    ){

        

        this.groupForm = fb.group(
            {
              id: [''],
              name: ['', Validators.required],
              icon: [''],
              employeePositionId: [''],
              zEmpRoleTypeId: [''],
              isContyStaff: 0,
              order: ''
            }
        );
    }
   
    ngOnInit(){
       if(this.group){
           this.groupForm.patchValue(this.group);
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
        if(this.group){

           this.service.updateGroup(this.group.id, this.groupForm.value).
            subscribe(
                res => {
                    this.group = <NavGroup> res;
                    this.groupForm.patchValue(this.group);
                    this.onFormSubmit.emit(this.group);
                }
            );
            
        }else{
            
            this.service.addGroup(this.groupForm.value, this.section).
                subscribe(
                    res => {
                        this.onFormSubmit.emit(<NavGroup>res);
                    }
                );        
        }
    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}