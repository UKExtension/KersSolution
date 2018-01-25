import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import {Location} from '@angular/common';
import { FormBuilder, Validators }   from '@angular/forms';
import {Router} from '@angular/router';
import {RolesService, Role} from '../roles/roles.service';
import { UsersService, Position } from '../users/users.service';
import {AdminNavigationService} from './admin-navigation.service';
import {  NavSection, NavGroup, NavItem } from '../../../components/reporting-navigation/navigation.service';

@Component({
    selector: 'navigation-item-form',
    templateUrl: 'navigation-item-form.component.html' 
})
export class NavigationItemFormComponent implements OnInit{

    itemForm = null;
    @Input() group:NavGroup = null;
    @Input() item:NavItem = null;
    errorMessage: string;

    roles: Role[];
    positions: Position[];

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<NavItem>();

    constructor( 
        private service: AdminNavigationService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location,
        private rolesService: RolesService,
        private usersService: UsersService
    ){

        

        this.itemForm = fb.group(
            {
              id: [''],
              name: ['', Validators.required],
              isRelative: [''],
              route: ['', Validators.required],
              employeePositionId: [''],
              zEmpRoleTypeId: [''],
              isContyStaff: 0,
              order: ''
            }
        );
    }
   
    ngOnInit(){
       if(this.item){
           this.itemForm.patchValue(this.item);
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
        if(this.item){

           this.service.updateItem(this.item.id, this.itemForm.value).
            subscribe(
                res => {
                    this.item = <NavItem> res;
                    this.itemForm.patchValue(this.item);
                    this.onFormSubmit.emit(this.item);
                }
            );
            
        }else{
            this.service.addItem(this.itemForm.value, this.group).
            subscribe(
                res => {
                    //this.group.items.push(<NavItem>res);
                    this.onFormSubmit.emit(<NavItem>res);
                }
            );     
        }
    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}