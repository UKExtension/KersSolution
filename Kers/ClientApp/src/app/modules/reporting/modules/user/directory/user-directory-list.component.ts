import { Component, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import { ProfileService } from '../../../components/reporting-profile/profile.service';
import { UserService, User, PlanningUnit } from '../user.service';
import "rxjs/add/operator/debounceTime";
import "rxjs/add/operator/switchMap";
import { Observable } from 'rxjs/Observable';





@Component({
    selector: 'user-directory-list',
    template: `
        <div class="row">


            <div class="clearfix"></div>

            <user-directory-profile *ngFor="let user of users | async" [user]="user" [showEmployeeSummaryButton]="showEmployeeSummaryButton" [showSnapButton]="onlySnapAssistants || onlyWitSnapCommitment"></user-directory-profile>
        </div>

        <div *ngIf="numProfiles != 0" class="text-center">
            <div>Showing {{ numProfiles }} of {{numResults}} Users</div>
            <div *ngIf="(users | async)?.length < numResults" class="btn btn-app" style="width: 97%; margin-right: 35px;" (click)="loadMore()">
                load more <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
        </div>
    `
})
export class UserDirectoryListComponent {


        @Input() county:PlanningUnit;
        @Input() onlyKSU:boolean=false;
        @Input() showEmployeeSummaryButton:boolean=false;
        @Input() onlySnapAssistants:boolean=false;
        @Input() onlyWitSnapCommitment:boolean=false;

        users: Observable<User[]>;
        errorMessage: string;
        planningUnits = null;
        positions = null;
        numResults = 0;
        numProfiles = 0;
        criteria = {
            search: '',
            position: 0,
            unit: 0,
            amount: 30,
            snapAssistants: 0,
            withSnapCommitment: 0,
            onlyKSU: 0
        }
        

        constructor(    
                    private service: UserService
                ){
                    
                }
   
    ngOnInit(){

        if(this.county != null){
            this.criteria.unit = this.county.id;
        }
        if(this.onlySnapAssistants ){
            this.criteria.snapAssistants = 1;
        }
        if(this.onlyWitSnapCommitment ){
            this.criteria.withSnapCommitment = 1;
        }
        if(this.onlyKSU ){
            this.criteria.onlyKSU = 1;
        }
        this.users = this.service.getCustom(this.criteria);

       
        
    }



    updateNumResults(){
        this.service.getCustomCount(this.criteria).subscribe(
            num => {
                this.numResults = num;
                this.numProfiles = Math.min(this.numResults, this.criteria.amount);
                return this.numResults;
            },
            error => this.errorMessage = <any> error
        );
        
    }
    loadMore(){
        this.criteria.amount = this.criteria.amount + 15;
    }


}