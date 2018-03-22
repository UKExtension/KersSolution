import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { PlansofworkService, Map, PlanOfWork, Plan } from './plansofwork.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: '[planofworkDetail]',
    templateUrl: 'plansofwork-detail.component.html'
})
export class PlansofworkDetailComponent implements OnInit {

    @Input ('planofworkDetail') planofwork: PlanOfWork;
    @Input ('isReport') isReport: boolean = false;

    @Output() onPlanofworkUpdated = new EventEmitter();
    @Output() onPlanofworkDeleted = new EventEmitter();

    editOppened = false;
    viewOppened = false;
    deleteOppened = false;
    deleteAllowed = false;
    rowOppened = true;
    errorMessage: string;
    fiscalYear:FiscalYear;
    plan:Plan;


    constructor(
        private plansofworkService: PlansofworkService){
        
    }

    ngOnInit(){
        this.plansofworkService.planForRevision(this.planofwork.id).subscribe(
            res => {
                this.plan = <Plan>res;
                this.fiscalYear = this.plan.fiscalYear;
            }
        );
        
    }
    edit(){
        this.rowOppened = false;
        this.editOppened = true;
    }
    view(){
        this.rowOppened = false;
        this.viewOppened = true;
    }
    delete(){
        this.rowOppened = false;
        this.deleteOppened = true;
    }
    confirmDelete(){
        this.plansofworkService.deletePlan(this.planofwork.id).subscribe(
            res => {
                    
                    this.onPlanofworkDeleted.emit();
                    return res;
                },
            error =>  this.errorMessage = <any>error
        );
    }
    close(){
        this.rowOppened = true;
        this.editOppened = false;
        this.viewOppened = false;
        this.deleteOppened = false;
    }
    mapUpdated(){
        this.onPlanofworkUpdated.emit();
        this.close();
    }
    mapDeleted(){
        this.onPlanofworkDeleted.emit();
    }

}