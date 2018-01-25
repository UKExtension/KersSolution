import {Component, OnInit} from '@angular/core';
import { RolesService, Role } from './roles.service';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {Router} from '@angular/router';
import { Observable } from 'rxjs/Observable';
import {ReportingRoleFormComponent} from './reporting-role-form.component';

@Component({
    template: `
<div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newRole" (click)="newRoleOpen()">+ new role</a>
    </div>
    <reporting-role-form *ngIf="newRole" (onFormCancel)="newRoleCancelled()" (onFormSubmit)="newRoleSubmitted()"></reporting-role-form>
</div>
<div *ngIf="roles">
    <table class="table table-striped">
        <thead>
        <tr>
            <th>Title</th>
            <th>Short Name</th>
            <th></th>
        </tr>
        </thead>
        <tbody>
                    <tr *ngFor="let role of roles" [rolesListDetail]="role" (onRoleUpdated)="onRoleUpdate()" (onRoleDeleted)="onRoleUpdate()"></tr>
        </tbody>               
    </table>            
</div>
       
    `

})
export class ReportingAdminRolesListComponent implements OnInit{

    roles: Role[] = null;
    errorMessage: string;
    newRole = false;
    previousTitle:string;

    constructor(
        private rolesService: RolesService,
        private reportingService: ReportingService,
        private router: Router
    ){}

    ngOnInit(){
        this.rolesService.listRoles().subscribe(
            roles => this.roles = roles,
            error =>  this.errorMessage = <any>error
        );
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Roles Management");
    }

    onRoleUpdate(){
        this.rolesService.listRoles().subscribe(
            roles => this.roles = roles,
            error =>  this.errorMessage = <any>error
        );
    }

    newRoleOpen(){
        this.newRole = true;
    }

    newRoleCancelled(){
        this.newRole=false;
    }
    newRoleSubmitted(){
        this.rolesService.listRoles().subscribe(
            roles => this.roles = roles,
            error =>  this.errorMessage = <any>error
        );
        this.newRole=false;
    }
}
