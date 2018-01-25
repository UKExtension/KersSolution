import { Component, ChangeDetectorRef, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {    AffirmativeService, 
            AffirmativePlan,
            MakeupDiversityGroup,
            MakeupValue,
            AdvisoryGroup,
            SummaryDiversity
    } from '../affirmative.service';
import { Router } from "@angular/router";
import {UserService, User} from '../../user/user.service';

@Component({
    selector: 'affirmative-report',
    templateUrl: 'affirmative-reporting-home.component.html'
})
export class AffirmativeReportsHomeComponent { 

    
    plan:AffirmativePlan;
    counter = 0;
    
    @Input() unitId:number = 0;

    makeupDiversityGroups: MakeupDiversityGroup[];
    advisoryGroups: AdvisoryGroup[];
    summaryDevirsity: SummaryDiversity[];
    values = [];

    user:User;
    errorMessage: string;

    constructor( 
        private reportingService: ReportingService,
        private service: AffirmativeService,
        private userService: UserService,
        private router: Router,
        private cdRef:ChangeDetectorRef
    )   
    {}

    ngOnInit(){
        this.counter = 0;
        this.defaultTitle();

        

        this.userService.current().subscribe(
            res => this.user = <User>res,
            err => this.errorMessage = <any>err
        );

        this.service.getMakeupDiversityGroups().subscribe(res => {
                this.makeupDiversityGroups = <MakeupDiversityGroup[]>res;
                this.service.getAdvisoryGroups().subscribe(res => {
                        this.advisoryGroups = res;
                        this.service.getSummaryDiversity().subscribe(res => {
                                this.summaryDevirsity = res;
                                this.service.get(this.unitId).subscribe(
                                    res=>{
                                        this.plan = <AffirmativePlan>res;
                                        for(var v of this.plan.makeupValues){
                                            this.values.push({val:"0"});
                                        }
                                    },
                                    err => this.errorMessage = <any> err
                                );
                                  
                            },
                            error =>  this.errorMessage = <any>error
                        );
                    },
                    error =>  this.errorMessage = <any>error
                );
            },
            error =>  this.errorMessage = <any>error
        );
        
    }

    cnt(type){
        if(this.counter == this.plan.makeupValues.length){
            this.counter = 0;
        }
        var v = this.plan.makeupValues[this.counter++].value;
        if(v == "" && type.type=="number") v="0";
        return v;
    }

    counterSummary = 0;

    cntSummary(){
        if(this.counterSummary == this.plan.summaryValues.length) this.counterSummary = 0;
        var vl =this.plan.summaryValues[this.counterSummary++].value;
        if(vl == "") vl="0";
        return vl;
    }


    defaultTitle(){
        this.reportingService.setTitle("Affirmative Action Plan for 2017-2018");
    }
}