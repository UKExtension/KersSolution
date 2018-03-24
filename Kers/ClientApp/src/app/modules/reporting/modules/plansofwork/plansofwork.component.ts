import { Component, OnInit, Input } from '@angular/core';
import {PlansofworkService, Map, PlanOfWork} from './plansofwork.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: 'plansofwork',
    templateUrl: 'plansofwork.component.html' 
})
export class PlansofworkComponent implements OnInit{

    @Input() fy:FiscalYear;

    plans:PlanOfWork[];
    newPlan = false;

    errorMessage: string;


    constructor(     
            private plansofworkService:PlansofworkService    
                ){
                    
                }
   
    ngOnInit(){
        this.plansofworkService.listPlans(this.fy.name).subscribe(
            plans => {
                this.plans = plans;
                
        },
            error =>  this.errorMessage = <any>error
        );
    }

    newPlanofworkOpen(){
        this.newPlan = true;
    }

    newPlanofworkCancelled(){
        this.newPlan = false;
    }

    newPlanofworkSubmitted(){
        this.newPlan = false;
    }

    onPlanofworkUpdate(){
        this.plansofworkService.listPlans(this.fy.name).subscribe(
            plans => this.plans = plans,
            error =>  this.errorMessage = <any>error
        );
    }
    

}