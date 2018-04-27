import { Component, OnInit, Input } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';
import {PlansofworkService, Map, PlanOfWork} from './plansofwork.service';
import { FiscalyearService, FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: 'plansofwork-reports',
    templateUrl: 'plansofwork-reports.component.html',
    styles: [`
        .active-year{
            font-weight: bold;
        }
    `] 
})
export class PlansofworkReportsComponent implements OnInit{

    @Input() planningUnitId:number = 0;
    plans:PlanOfWork[];

    fiscalYears: FiscalYear[];
    nextFiscalYear: FiscalYear;
    currentFiscalYear: FiscalYear;
    selectedFiscalYear: FiscalYear;

    errorMessage: string;


    constructor(     
            private plansofworkService:PlansofworkService,
            private reportingService: ReportingService   
                ){
                    
                }
   
    ngOnInit(){

    }


    selectFiscalYear(year:FiscalYear){
        this.selectedFiscalYear = year;
        if(this.planningUnitId == 0){
            this.defaultTitle();
        }
        this.getList();        
    }



    getList(){
        this.plansofworkService.listPlansDetails(this.selectedFiscalYear.name, this.planningUnitId).subscribe(
            plans => {
                this.plans = plans;
                
        },
            error =>  this.errorMessage = <any>error
        );
    }


    defaultTitle(){
        this.reportingService.setTitle("Plans of Work Report for FY "+this.selectedFiscalYear.name);
    }
    

}