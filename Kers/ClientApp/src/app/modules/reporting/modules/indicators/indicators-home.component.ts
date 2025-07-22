import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import {IndicatorsService, Indicator} from './indicators.service';
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';

@Component({
  template: `


    These numbers are to be kept up to date PER INDIVIDUAL (YOU) - NOT THE COUNTY.<br>
Simply update the numbers as needed throughout the fiscal year.<br>
<strong>ENTER WHOLE NUMBERS ONLY.</strong><br>
<div *ngIf="fiscalYear" style="padding-top:15px;">
    <indicators-form [fiscalYear]="fiscalYear"></indicators-form>
</div>
  `
})
export class IndicatorsHomeComponent { 
    
    fiscalYear:FiscalYear;
    constructor( 
        private reportingService: ReportingService, 
        private fiscalYearService:FiscalyearService
    )   
    {
       
    }

    ngOnInit(){
        this.fiscalYearService.current("serviceLog", true).subscribe(
            res => {
                this.fiscalYear = res;
                this.defaultTitle();
            }
        )
    }
    
    

    defaultTitle(){
        this.reportingService.setTitle("Indicators for FY"+this.fiscalYear.name);
    }
}