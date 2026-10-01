import {Component, Input, Output, EventEmitter, ChangeDetectionStrategy} from '@angular/core';
import { StrategicInitiative, MajorProgram} from '../programs/programs.service';



@Component({
    selector: 'programindicators-initiative-detail',
    template: `
    <div class="col-xs-10">@if (!programs) {
      <span>{{initiative.name}}</span>
      }@if (programs) {
      <strong>{{initiative.name}}</strong>
    }
    @if (programs) {
      <div class="col-xs-12">
        <programindicators-programs-admin [programs]="initiative.majorPrograms"></programindicators-programs-admin>
      </div>
    }
    </div>
    <div class="col-xs-2">
      @if (!programs) {
        <a class="btn btn-info btn-xs" (click)="programs=!programs">programs</a>
      }
      @if (programs) {
        <a class="btn btn-info btn-xs" (click)="programs=!programs">close</a>
      }
    </div>
    `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ProgramindicatorsInitiativeDetailComponent{
    
    @Input()initiative:StrategicInitiative; 
    programs = false;

    constructor(
       
    ){}

    ngOnInit(){
        
    }
    
    

}