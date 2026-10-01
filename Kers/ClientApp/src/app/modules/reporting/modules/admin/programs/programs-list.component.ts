import {Component, OnInit, Input, Output, EventEmitter, ChangeDetectionStrategy} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs';
import {InitiativeFormComponent} from './initiative-form.component';
import {ProgramsService, StrategicInitiative, MajorProgram} from './programs.service';

@Component({
    selector: 'programs-list',
    template: `
<div>
  <div class="text-right">
    @if (!newProgram) {
      <a class="btn btn-info btn-xs" (click)="newProgramOpen()">+ new major program</a>
    }
  </div>
  @if (newProgram) {
    <program-form [initiative]="initiative" (onFormCancel)="newProgramCancelled()" (onFormSubmit)="newProgramSubmitted()"></program-form>
  }
</div>
@if (programs) {
  <div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th>Name</th>
          <th>Code</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        @for (program of programs; track program) {
          <tr [programListDetail]="program" (onProgramUpdated)="onProgramUpdate()" (onProgramDeleted)="onProgramUpdate()"></tr>
        }
      </tbody>
    </table>
  </div>
}

`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
