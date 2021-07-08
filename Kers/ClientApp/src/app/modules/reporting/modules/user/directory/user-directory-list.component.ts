import { Component, Input } from '@angular/core';
import { UserService, User, PlanningUnit } from '../user.service';
import { Observable } from 'rxjs';





@Component({
    selector: 'user-directory-list',
    template: `
        <div class="row">
            <div class="text-right" *ngIf="switchEnabled" style="padding: 10px 0;">
                <span style="vertical-align:top;">Include Former CES Employees</span> <label class="switch">
                    <input type="checkbox" id="onlyEnabledCheckbox">
                    <div class="slider round" (click)="includeLeftChecked()"></div>
                </label>
            
            </div>

            <div class="clearfix"></div>

            <user-directory-profile *ngFor="let user of users | async" [user]="user" [showEmployeeSummaryButton]="showEmployeeSummaryButton" [showSnapButton]="showSnapButton"></user-directory-profile>
        </div>

        <div *ngIf="numProfiles != 0" class="text-center">
            <div>Showing {{ numProfiles }} of {{numResults}} Users</div>
            <div *ngIf="(users | async)?.length < numResults" class="btn btn-app" style="width: 97%; margin-right: 35px;" (click)="loadMore()">
                load more <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
        </div>
    `,
    styles: [
        `
        .switch {
          position: relative;
          display: inline-block;
          width: 30px;
          height: 17px;
        }
        
        /* Hide default HTML checkbox */
        .switch input {display:none;}
        
        /* The slider */
        .slider {
          position: absolute;
          cursor: pointer;
          top: 0;
          left: 0;
          right: 0;
          bottom: 0;
          background-color: #ccc;
          -webkit-transition: .4s;
          transition: .4s;
        }
        
        .slider:before {
          position: absolute;
          content: "";
          height: 13px;
          width: 13px;
          left: 2px;
          bottom: 2px;
          background-color: white;
          -webkit-transition: .4s;
          transition: .4s;
        }
        
        input:checked + .slider {
          background-color: rgb(38, 185, 154);
          border-color: rgb(38, 185, 154); 
          box-shadow: rgb(38, 185, 154) 
        }
        
        input:focus + .slider {
          box-shadow: 0 0 1px rgb(38, 185, 154);
        }
        
        input:checked + .slider:before {
          -webkit-transform: translateX(13px);
          -ms-transform: translateX(13px);
          transform: translateX(13px);
        }
        
        /* Rounded sliders */
        .slider.round {
          border-radius: 17px;
        }
        
        .slider.round:before {
          border-radius: 50%;
        }
        
        `
    ]
})
export class UserDirectoryListComponent {


        @Input() county:PlanningUnit;
        @Input() onlyKSU:boolean=false;
        @Input() showEmployeeSummaryButton:boolean=false;
        @Input() onlySnapAssistants:boolean=false;
        @Input() onlyWitSnapCommitment:boolean=false;
        @Input() switchEnabled:boolean=true;
        @Input() showSnapButton:boolean=false;
        @Input() initialAmount:number=30;

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
            amount: this.initialAmount,
            snapAssistants: 0,
            withSnapCommitment: 0,
            onlyKSU: 0,
            enabled: 1
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
        this.criteria.amount = this.initialAmount;
        this.users = this.service.getCustom(this.criteria);

       
        
    }

    includeLeftChecked(){
        if(this.criteria.enabled == 0){
            this.criteria.enabled = 1;
        }else{
            this.criteria.enabled = 0;
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