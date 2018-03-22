import { Component, Input } from '@angular/core';
import { PlansofworkService, PlanOfWork, Plan, PlanningUnit } from './plansofwork.service';
import { Observable } from "rxjs/Observable";

@Component({
    selector: 'plansofwork-view',
    templateUrl: 'plansofwork-view.component.html' 
})
export class PlansofworkViewComponent{

    @Input ('planofwork') plan: PlanOfWork;

    public thePlan: Plan
    errorMessage: string;

    constructor(
        private service:PlansofworkService
    ){

    }

    ngOnInit(){
        
        this.service.planForRevision(this.plan.id).subscribe(
            res=>{
                this.thePlan = <Plan>res;                
                },
            error => this.errorMessage = <any>error
        );
    }

}