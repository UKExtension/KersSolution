import { Component, Input } from '@angular/core';
import { PlansofworkService, PlanOfWork, Plan, PlanningUnit } from '../plansofwork/plansofwork.service';
import { Observable } from 'rxjs';
import { DistrictService, County } from "./district.service";
import {ReportingService} from '../../components/reporting/reporting.service';

@Component({
    selector: 'district-plansofwork',
    templateUrl: 'district-plans.component.html' 
})
export class DistrictPlansComponent{

    counties:Observable<County[]>;
    errorMessage: string;
    plans:Observable<PlanOfWork[]>;

    constructor(
        private plansService:PlansofworkService,
        private service:DistrictService,
        private reportingService: ReportingService 
    ){

    }

    ngOnInit(){
        
        this.counties = this.service.mycounties(); 
        this.defaultTitle();
    }

    onChange(countyId){
        this.plans = this.plansService.plansForCounty(countyId);
    }
    defaultTitle(){
        this.reportingService.setTitle("District Report");
    }

}