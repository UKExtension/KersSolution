import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import {IndicatorsService, Indicator} from '../indicators/indicators.service';
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';

@Component({
  template: `
<br><br>
<span style="color:orange;">---- This form serves demonstration purpose and won't work or save entered test data. ----</span><br><br>
    These numbers are to be kept up to date PER INDIVIDUAL (YOU) - NOT THE COUNTY.<br>
Simply update the numbers as needed throughout the fiscal year.<br>
<strong>ENTER WHOLE NUMBERS ONLY.</strong><br>
<div *ngIf="fiscalYear" style="padding-top:15px;">
    <indicators-form [fiscalYear]="fiscalYear" [demoMode]="true"></indicators-form>
</div>
  `
})
export class IndicatorsDemoComponent { 
    
    fiscalYear:FiscalYear;
    constructor( 
        private reportingService: ReportingService, 
        private fiscalYearService:FiscalyearService
    )   
    {
       
    }

    ngOnInit(){
        this.fiscalYearService.next("serviceLog", true).subscribe(
            res => {
                this.fiscalYear = res;
                this.defaultTitle();
            }
        )
    }
    
    

    defaultTitle(){
        this.reportingService.setTitle("Program Indicators for FY"+this.fiscalYear.name);
    }
}