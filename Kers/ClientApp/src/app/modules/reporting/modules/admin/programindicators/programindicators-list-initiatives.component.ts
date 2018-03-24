import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { ProgramsService, StrategicInitiative, MajorProgram} from '../programs/programs.service';
import { FiscalyearService, FiscalYear } from '../fiscalyear/fiscalyear.service';



@Component({
    selector: 'programindicators-initiatives-admin',
    templateUrl: `programindicators-list-initiatives.component.html`,
    styles: [`
        .row{
            padding-top: 20px;
            padding-bottom: 20px;
            border-bottom: 1px solid #D9DEE4;
            margin: 0;
        }
    `]
})
export class ProgramindicatorsListInitiativesComponent implements OnInit{
    
    public initiatives:Observable<StrategicInitiative[]>; 
    selectedFiscalYear: FiscalYear;

    constructor(
        private programsService:ProgramsService,
        private fiscalYearService: FiscalyearService
    ){}
    
    ngOnInit(){
    
    }

    
    selectFiscalYear(year:FiscalYear){
        if(this.selectedFiscalYear == null || this.selectedFiscalYear.id != year.id){
            this.selectedFiscalYear = year;
            this.initiatives = this.programsService.listInitiatives(this.selectedFiscalYear.name);
        }
        
    }
    
    
    

}