import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';
import {PlansofworkService, Map, PlanOfWork} from './plansofwork.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import { Observable } from 'rxjs';

@Component({
    selector: 'plansofwork',
    templateUrl: 'plansofwork.component.html',
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class PlansofworkComponent implements OnInit{

    @Input() fy:FiscalYear;

    plans$:Observable<PlanOfWork[]>;
    plans:PlanOfWork[];
    newPlan = false;

    errorMessage: string;


    constructor(     
            private plansofworkService:PlansofworkService    
                ){
                    
                }
   
    ngOnInit(){
         this.plansofworkService.listPlans(this.fy.name).subscribe(
            res => this.plans = res
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
        this.plansofworkService.listPlans(this.fy.name).subscribe(
            res => this.plans = res
        );
    }

    onPlanofworkUpdate(){
        this.plansofworkService.listPlans(this.fy.name).subscribe(
            res => this.plans = res
        );
    }
    

}