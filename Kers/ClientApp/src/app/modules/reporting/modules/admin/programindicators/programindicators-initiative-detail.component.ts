import {Component, Input, Output, EventEmitter} from '@angular/core';
import { StrategicInitiative, MajorProgram} from '../programs/programs.service';



@Component({
    selector: 'programindicators-initiative-detail',
    template: `
    <div class="col-xs-10"><span *ngIf="!programs">{{initiative.name}}</span><strong *ngIf="programs">{{initiative.name}}</strong>
        <div class="col-xs-12" *ngIf="programs">
            <programindicators-programs-admin [programs]="initiative.majorPrograms"></programindicators-programs-admin>
        </div>
    </div>
    <div class="col-xs-2">
        <a class="btn btn-info btn-xs" (click)="programs=!programs" *ngIf="!programs">programs</a>
        <a class="btn btn-info btn-xs" (click)="programs=!programs" *ngIf="programs">close</a>
    </div>
    `
})
export class ProgramindicatorsInitiativeDetailComponent{
    
    @Input()initiative:StrategicInitiative; 

    constructor(
       
    ){}

    ngOnInit(){
        
    }
    
    

}