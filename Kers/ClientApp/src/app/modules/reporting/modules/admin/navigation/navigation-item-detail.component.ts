import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';

import { NavigationService, NavItem, NavGroup } from '../../../components/reporting-navigation/navigation.service';
import {UsersService, Position} from '../users/users.service';
import {Role, RolesService} from '../roles/roles.service';
import {AdminNavigationService} from './admin-navigation.service';

@Component({
    selector: '[navigationItemDetail]',
    templateUrl: 'navigation-item-detail.component.html'
})
export class NavigationItemDetailComponent implements OnInit {

    @Input ('navigationItemDetail') item: NavItem;

    @Output() onItemUpdated = new EventEmitter<NavItem>();
    @Output() onItemDeleted = new EventEmitter<NavItem>();
    

    editOppened = false;
    deleteOppened = false;
    rowOppened = true;
    errorMessage: string;

    positionRestriction = "";
    roleRestriction = "";
    

    constructor(
        private navigationService: NavigationService,
        private service: AdminNavigationService,
        private usersService:UsersService,
        private rolesService: RolesService
        ){
        
    }

    ngOnInit(){
        if(this.item.employeePositionId != null){
            this.usersService.positions().subscribe(
                res => {
                    var positions = <Position[]>res;
                    this.positionRestriction = ' ('+positions.filter(p => p.id == this.item.employeePositionId)[0].title + ')';
                },
                err => this.errorMessage = <any>err
            );
        }
        if(this.item.zEmpRoleTypeId != null && this.item.zEmpRoleTypeId != 0){
            this.rolesService.listRoles().subscribe(
                res => {
                    var roles = <Role[]>res;
                    this.roleRestriction = ' (' + roles.filter(r => r.id == this.item.zEmpRoleTypeId)[0].title + ')';
                },
                err => this.errorMessage = <any> err
            )
        }
    }

    edit(){
        this.rowOppened = false;
        this.editOppened = true;
    }
    delete(){
        this.rowOppened = false;
        this.deleteOppened = true;
    }
    items(){
        this.rowOppened = false;
    }
    onUpdateSubmit(event){
        this.item = <NavItem> event;
        this.onItemUpdated.emit(this.item);
        this.close();
    }
    confirmDelete(){
        this.service.deleteItem(this.item).subscribe(
            res => {
                    
                    this.onItemDeleted.emit(this.item);
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
    
    
}
