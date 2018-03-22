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
        .active-year{
            font-weight: bold;
        }
    `]
})
export class ProgramindicatorsListInitiativesComponent implements OnInit{
    
    public initiatives:Observable<StrategicInitiative[]>; 
    fiscalYears: FiscalYear[];
    nextFiscalYear: FiscalYear;
    currentFiscalYear: FiscalYear;
    selectedFiscalYear: FiscalYear;

    constructor(
        private programsService:ProgramsService,
        private fiscalYearService: FiscalyearService
    ){}
    
    ngOnInit(){
        this.getNextFiscalYear();
    }

    getNextFiscalYear(){
        this.fiscalYearService.next("serviceLog").subscribe(
            res => {
                this.nextFiscalYear = res;
                if(this.nextFiscalYear == null){
                    this.getCurrentFiscalYear();
                }else{
                    this.selectedFiscalYear = this.nextFiscalYear;
                    this.getFiscalYears();                    
                }
            }
        );
    }
    getCurrentFiscalYear(){
        this.fiscalYearService.current("serviceLog").subscribe(
            res => {
                this.currentFiscalYear = res;
                if(this.nextFiscalYear != null){
                    this.selectedFiscalYear = this.currentFiscalYear;
                    this.getFiscalYears();                    
                }
            }
        );
    }

    getFiscalYears(){
        this.fiscalYearService.byType("serviceLog").subscribe(
            res => {
                this.fiscalYears = res;
                this.initiatives = this.programsService.listInitiatives(this.selectedFiscalYear.name);
            }
        );
    }
    selectFiscalYear(year:FiscalYear){
        if(this.selectedFiscalYear.id != year.id){
            this.selectedFiscalYear = year;
            this.initiatives = this.programsService.listInitiatives(this.selectedFiscalYear.name);
        }
        
    }
    
    
    

}