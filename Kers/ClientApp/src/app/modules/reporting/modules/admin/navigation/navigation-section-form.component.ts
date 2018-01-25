import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import {Location} from '@angular/common';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';
import {AdminNavigationService} from './admin-navigation.service';
import {  NavSection } from '../../../components/reporting-navigation/navigation.service';
import {RolesService, Role} from '../roles/roles.service';
import { UsersService, Position } from '../users/users.service';


@Component({
    selector: 'navigation-section-form',
    templateUrl: 'navigation-section-form.component.html' 
})
export class NavigationSectionFormComponent implements OnInit{

    sectionForm = null;
    @Input() section:NavSection = null;
    errorMessage: string;

    roles: Role[];
    positions: Position[];

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private reportingService: ReportingService,
        private service: AdminNavigationService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location,
        private rolesService: RolesService,
        private usersService: UsersService
    ){

        

        this.sectionForm = fb.group(
            {
              name: ['', Validators.required],
              employeePositionId: [''],
              zEmpRoleTypeId: [''],
              isContyStaff: 0
            }
        );
    }
   
    ngOnInit(){
       if(this.section){
           this.sectionForm.patchValue(this.section);
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
        if(this.section){

           this.service.updateSection(this.section.id, this.sectionForm.value).
            subscribe(
                res => {
                    this.section = <NavSection> res;
                    this.sectionForm.patchValue(this.section);
                    this.onFormSubmit.emit();
                }
            );
            
        }else{
            
            this.service.addSection(this.sectionForm.value).
            subscribe(
                res => {
                    this.onFormSubmit.emit();
                }
            );
            this.onFormSubmit.emit();   
        
        }
    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}