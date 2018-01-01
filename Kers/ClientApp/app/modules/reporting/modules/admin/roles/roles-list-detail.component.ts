import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { RolesService, Role } from './roles.service';

@Component({
    selector: '[rolesListDetail]',
    templateUrl: 'roles-list-detail.component.html'
})
export class RolesListDetailComponent implements OnInit {

    @Input ('rolesListDetail') role: Role;

    @Output() onRoleUpdated = new EventEmitter();
    @Output() onRoleDeleted = new EventEmitter();

    editOppened = false;
    deleteOppened = false;
    rowOppened = true;
    errorMessage: string;


    constructor(private rolesService: RolesService){
        
    }

    ngOnInit(){
        
    }
    edit(){
        this.rowOppened = false;
        this.editOppened = true;
    }
    delete(){
        this.rowOppened = false;
        this.deleteOppened = true;
    }
    confirmDelete(){
        this.rolesService.deleteRole(this.role.id).subscribe(
            res => {
                    
                    this.onRoleDeleted.emit();
                    return res;
                },
            error =>  this.errorMessage = <any>error
        );
    }
    close(){
        this.rowOppened = true;
        this.editOppened = false;
        this.deleteOppened = false;
    }
    roleUpdated(){
        this.onRoleUpdated.emit();
        this.close();
    }
    roleDeleted(){
        this.onRoleDeleted.emit();
    }

}