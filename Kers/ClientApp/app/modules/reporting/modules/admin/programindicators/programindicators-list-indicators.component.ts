import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { ProgramsService, StrategicInitiative, MajorProgram} from '../programs/programs.service';
import {IndicatorsService, Indicator} from '../../indicators/indicators.service';


@Component({
    selector: 'programindicators-list-indicators',
    templateUrl: `programindicators-list-indicators.component.html`,
    styles: [`
        .row{
            padding-top: 10px;
            padding-bottom: 10px;
            border-bottom: none;
        }
    `]
})
export class ProgramindicatorsListIndicatorsComponent implements OnInit{
    
    
    @Input()majorProgram:MajorProgram;
    newIndicator = false;
    indicators:Indicator[];

    errorMessage:string;

    constructor(
        private indicatorsService:IndicatorsService
    ){}
    
    
    
    ngOnInit(){
        this.indicatorsService.listIndicators(this.majorProgram).subscribe(
            res => this.indicators = <Indicator[]> res,
            err => this.errorMessage = <any>err
        );
    }

    onNewIndicator(indicator:Indicator){
        this.indicators.push(indicator);
        this.newIndicator = false;
    }

    indicatorDeleted(indicator:Indicator){
        let index: number = this.indicators.indexOf(indicator);
        if (index !== -1) {
            this.indicators.splice(index, 1);
        }
    }

}