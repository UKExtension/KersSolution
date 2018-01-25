import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs/Observable';
import {InitiativeFormComponent} from './initiative-form.component';
import {ProgramsService, StrategicInitiative, MajorProgram} from './programs.service';

@Component({
    selector: 'programs-list',
    template: `
<div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newProgram" (click)="newProgramOpen()">+ new major program</a>
    </div>
    <program-form *ngIf="newProgram" [initiative]="initiative" (onFormCancel)="newProgramCancelled()" (onFormSubmit)="newProgramSubmitted()"></program-form>
</div>
<div *ngIf="programs">
    <table class="table table-striped">
        <thead>
        <tr>
            <th>Name</th>
            <th>Code</th>
            <th></th>
        </tr>
        </thead>
        <tbody>
            <tr *ngFor="let program of programs" [programListDetail]="program" (onProgramUpdated)="onProgramUpdate()" (onProgramDeleted)="onProgramUpdate()"></tr>
        </tbody>               
    </table>            
</div>
       
    `

})
export class ProgramsListComponent implements OnInit{


    errorMessage: string;
    newProgram = false;

    programs: MajorProgram[];
    @Input() initiative: StrategicInitiative;

    @Output() onChange = new EventEmitter<void>();

    constructor(
        private router: Router,
        private service: ProgramsService
    ){}

    ngOnInit(){
        this.programs = this.initiative.majorPrograms;
    }


    newProgramOpen(){
        this.newProgram = true;
    }

    newProgramCancelled(){
        this.newProgram=false;
    }
    newProgramSubmitted(){
        this.newProgram=false;
        this.onChange.emit();
    }

    onProgramUpdate(){
        this.onChange.emit();
    }
}
