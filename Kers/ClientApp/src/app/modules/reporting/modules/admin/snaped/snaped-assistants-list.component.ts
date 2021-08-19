import { Component } from '@angular/core';
import { SnapedAdminService } from './snaped-admin.service';
import { FiscalyearService, FiscalYear } from '../fiscalyear/fiscalyear.service';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { ActivatedRoute, Params } from '@angular/router';
import { PlanningUnit, User } from '../../user/user.service';
import { PlanningunitService } from '../../planningunit/planningunit.service';
import { Observable } from 'rxjs';

@Component({
    selector: 'snaped-assistants-list',
    template: `
        <span *ngFor="let assistant of assistants | async"><a [routerLink]="['/reporting/admin/snaped/user', assistant.id]">{{assistant.personalProfile.firstName}} {{assistant.personalProfile.lastName}}</a>&nbsp;|&nbsp;</span>
    `
})
export class SnapedAssistantsListComponent { 




    assistants: Observable<User[]>;

    errorMessage: string;

    constructor( 
        private service:SnapedAdminService,
    )   
    {}

    ngOnInit(){

        this.assistants = this.service.assistants();





    }


}