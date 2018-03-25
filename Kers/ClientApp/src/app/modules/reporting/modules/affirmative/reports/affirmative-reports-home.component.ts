import { Component, ChangeDetectorRef, Input } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';
import {    AffirmativeService, 
            AffirmativePlan,
            MakeupDiversityGroup,
            MakeupValue,
            AdvisoryGroup,
            SummaryDiversity,
            SummaryValue
    } from '../affirmative.service';
import { Router } from "@angular/router";
import {UserService, User} from '../../user/user.service';
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: 'affirmative-report',
    templateUrl: 'affirmative-reporting-home.component.html'
})
export class AffirmativeReportsHomeComponent { 

    
    plan:AffirmativePlan;
    counter = 0;
    loading = true;
    
    @Input() unitId:number = 0;

    makeupDiversityGroups: MakeupDiversityGroup[];
    advisoryGroups: AdvisoryGroup[];
    summaryDevirsity: SummaryDiversity[];
    values = [];

    user:User;
    errorMessage: string;

    selectedFiscalYear:FiscalYear;

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

        this.userService.current().subscribe(
            res => this.user = <User>res,
            err => this.errorMessage = <any>err
        );

        
        
    }

    getPlan(){
        this.loading = true;
        this.service.get(this.unitId, this.selectedFiscalYear.name).subscribe(
            res=>{
                this.plan = <AffirmativePlan>res;
                if(this.plan != null){
                    for(var v of this.plan.makeupValues){
                        this.values.push({val:"0"});
                    }
                }
                this.loading = false;
            },
            err => this.errorMessage = <any> err
        );
    }

    setValues(){
        this.service.getMakeupDiversityGroups().subscribe(res => {
            this.makeupDiversityGroups = <MakeupDiversityGroup[]>res;
            this.service.getAdvisoryGroups().subscribe(res => {
                    this.advisoryGroups = res;
                    this.service.getSummaryDiversity().subscribe(res => {
                            this.summaryDevirsity = res;
                            this.getPlan();
                              
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
        
        var vl = this.plan.summaryValues[this.counterSummary++];
        if(vl.value == undefined){
            return 0;
        }else{
            return +vl.value;
        }
        
    }


    selectFiscalYear(event:FiscalYear){
        if(this.selectedFiscalYear == null ){
            this.selectedFiscalYear = event;
            this.setValues();
            this.defaultTitle();
        }else if(this.selectedFiscalYear.id != event.id){
            this.selectedFiscalYear = event;
            this.getPlan();
            this.defaultTitle();
        }

    }
    yearsLoaded(event:FiscalYear[]){

    }


    defaultTitle(){
        this.reportingService.setTitle("Affirmative Action Plan for FY " + this.selectedFiscalYear.name);
    }
}