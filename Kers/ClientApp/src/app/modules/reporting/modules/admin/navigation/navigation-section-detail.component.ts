import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';

import { NavigationService, NavSection, NavGroup } from '../../../components/reporting-navigation/navigation.service';
import {UsersService, Position} from '../users/users.service';
import {Role, RolesService} from '../roles/roles.service';
import {AdminNavigationService} from './admin-navigation.service';

@Component({
    selector: '[navigationSectionDetail]',
    templateUrl: 'navigation-section-detail.component.html'
})
export class NavigationSectionDetailComponent implements OnInit {

    @Input ('navigationSectionDetail') section: NavSection;

    @Output() onSectionUpdated = new EventEmitter();
    @Output() onSectionDeleted = new EventEmitter();
    

    editOppened = false;
    deleteOppened = false;
    groupsOppened = false;
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
        if(this.section.employeePositionId != null){
            this.usersService.positions().subscribe(
                res => {
                    var positions = <Position[]>res;
                    this.positionRestriction = ' ('+positions.filter(p => p.id == this.section.employeePositionId)[0].title + ')';
                },
                err => this.errorMessage = <any>err
            );
        }
        if(this.section.zEmpRoleTypeId != null && this.section.zEmpRoleTypeId != 0){
            this.rolesService.listRoles().subscribe(
                res => {
                    var roles = <Role[]>res;
                    this.roleRestriction = ' (' + roles.filter(r => r.id == this.section.zEmpRoleTypeId)[0].title + ')';
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
    groups(){
        this.rowOppened = false;
        this.groupsOppened = true;
    }
    onUpdateSubmit(){
        this.onSectionUpdated.emit();
        this.close();
    }
    confirmDelete(){
        this.service.deleteSection(this.section).subscribe(
            res => {
                    
                    this.onSectionDeleted.emit();
                    return res;
                },
            error =>  this.errorMessage = <any>error
        );
        
    }
    close(){
        this.rowOppened = true;
        this.editOppened = false;
        this.deleteOppened = false;
        this.groupsOppened = false;
    }
    
    groupAdded( event){
        this.section.groups.push(event);
    }
}
