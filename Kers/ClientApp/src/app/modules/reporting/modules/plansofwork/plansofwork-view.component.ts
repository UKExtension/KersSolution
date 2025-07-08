import { Component, Input } from '@angular/core';
import { PlansofworkService, PlanOfWork, Plan, PlanningUnit } from './plansofwork.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: 'plansofwork-view',
    templateUrl: 'plansofwork-view.component.html' 
})
export class PlansofworkViewComponent{

    @Input ('planofwork') plan: PlanOfWork;

    public thePlan: Plan
    errorMessage: string;
    fiscalYear:FiscalYear;

    constructor(
        private service:PlansofworkService
    ){

    }

    ngOnInit(){
        
        this.service.planForRevision(this.plan.id).subscribe(
            res=>{
                this.thePlan = <Plan>res;
                this.fiscalYear = this.thePlan.fiscalYear;               
                },
            error => this.errorMessage = <any>error
        );
    }

}