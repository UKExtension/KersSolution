import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';

import { NavigationService, NavSection, NavGroup, NavItem } from '../../../components/reporting-navigation/navigation.service';
import {UsersService, Position} from '../users/users.service';
import {Role, RolesService} from '../roles/roles.service';
import {AdminNavigationService} from './admin-navigation.service';

@Component({
    selector: '[navigationGroupDetail]',
    templateUrl: 'navigation-group-detail.component.html'
})
export class NavigationGroupDetailComponent implements OnInit {

    @Input ('navigationGroupDetail') group: NavGroup;

    @Output() onGroupUpdated = new EventEmitter<NavGroup>();
    @Output() onGroupDeleted = new EventEmitter<NavGroup>();
    

    

    editOppened = false;
    deleteOppened = false;
    itemsOppened = false;
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
        if(this.group.employeePositionId != null){
            this.usersService.positions().subscribe(
                res => {
                    var positions = <Position[]>res;
                    this.positionRestriction = ' ('+positions.filter(p => p.id == this.group.employeePositionId)[0].title + ')';
                },
                err => this.errorMessage = <any>err
            );
        }
        if(this.group.zEmpRoleTypeId != null && this.group.zEmpRoleTypeId != 0){
            this.rolesService.listRoles().subscribe(
                res => {
                    var roles = <Role[]>res;
                    this.roleRestriction = ' (' + roles.filter(r => r.id == this.group.zEmpRoleTypeId)[0].title + ')';
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
        this.itemsOppened = true;
    }
    onUpdateSubmit(event){
        this.group = <NavGroup> event;
        this.onGroupUpdated.emit(this.group);
        this.close();
    }
    confirmDelete(){
        this.service.deleteGroup(this.group).subscribe(
            res => {
                    
                    this.onGroupDeleted.emit(this.group);
                    return res;
                },
            error =>  this.errorMessage = <any>error
        );
        
    }
    close(){
        this.rowOppened = true;
        this.editOppened = false;
        this.deleteOppened = false;
        this.itemsOppened = false;
    }
    newItem(event){
        if(this.group.items == undefined){
            this.group.items = [event];
        }else{
            this.group.items.push(event);
        }
        this.onGroupUpdated.emit(this.group);
        
    }

    itemDeleted(event:NavItem){
        let index: number = this.group.items.indexOf(event);
        if (index !== -1) {
            this.group.items.splice(index, 1);
        }
        this.onGroupUpdated.emit(this.group);
    }
    
    
}
