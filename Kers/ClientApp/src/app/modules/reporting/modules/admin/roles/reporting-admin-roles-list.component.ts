import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import { RolesService, Role } from './roles.service';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {Router} from '@angular/router';
import { Observable } from 'rxjs';
import {ReportingRoleFormComponent} from './reporting-role-form.component';

@Component({
    template: `
<div>
  <div class="text-right">
    @if (!newRole) {
      <a class="btn btn-info btn-xs" (click)="newRoleOpen()">+ new role</a>
    }
  </div>
  @if (newRole) {
    <reporting-role-form (onFormCancel)="newRoleCancelled()" (onFormSubmit)="newRoleSubmitted()"></reporting-role-form>
  }
</div>
@if (roles) {
  <div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th>Title</th>
          <th>Short Name</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        @for (role of roles; track role) {
          <tr [rolesListDetail]="role" (onRoleUpdated)="onRoleUpdate()" (onRoleDeleted)="onRoleUpdate()"></tr>
        }
      </tbody>
    </table>
  </div>
}

`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
