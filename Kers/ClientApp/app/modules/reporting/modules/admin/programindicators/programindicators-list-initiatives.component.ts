import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { ProgramsService, StrategicInitiative, MajorProgram} from '../programs/programs.service';



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

    constructor(
        private programsService:ProgramsService
    ){}
    
    
    
    ngOnInit(){
        this.initiatives = this.programsService.listInitiatives();
    }

}