import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs';
import {InitiativeFormComponent} from './initiative-form.component';
import {ProgramsService, StrategicInitiative, MajorProgram} from './programs.service';
import { FiscalyearService, FiscalYear } from '../fiscalyear/fiscalyear.service';

@Component({
    template: `
    <fiscal-year-switcher (onSwitched)="selectFiscalYear($event)"></fiscal-year-switcher><br><br>
    <div>
      <div class="text-right">
        @if (!newInitiative) {
          <a class="btn btn-info btn-xs" (click)="newInitiativeOpen()">+ new statigic initiative</a>
        }
      </div>
      @if (newInitiative) {
        <initiative-form (onFormCancel)="newInitiativeCancelled()" (onFormSubmit)="newInitiativeSubmitted()" [fiscalYear]="selectedFiscalYear"></initiative-form>
      }
    </div>
    @if (initiatives) {
      <div>
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Name</th>
              <th>Category</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            @for (initiative of initiatives; track initiative) {
              <tr [initiativeListDetail]="initiative" (onInitiativeUpdated)="onInitiativeUpdate()" (onInitiativeDeleted)="onInitiativeUpdate()"></tr>
            }
          </tbody>
        </table>
      </div>
    }
    
    `,
    styles: [`
        .active-year{
            font-weight: bold;
        }
    `],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class InitiativeListComponent implements OnInit{


    errorMessage: string;
    newInitiative = false;

    initiatives: StrategicInitiative[];

    selectedFiscalYear: FiscalYear;

    constructor(
        private router: Router,
        private service: ProgramsService
    ){}

    ngOnInit(){
        
    }
    
    selectFiscalYear(year:FiscalYear){
        if(this.selectedFiscalYear == null || this.selectedFiscalYear.id != year.id){
            this.selectedFiscalYear = year;
            this.getList()
        }
    }

    getList(){
        
        this.service.listInitiatives(this.selectedFiscalYear.name, false).subscribe(
            i => this.initiatives = i,
            error =>  this.errorMessage = <any>error
        );
        
    }

    onInitiativeUpdate(){
        this.getList();
    }

    newInitiativeOpen(){
        this.newInitiative = true;
    }

    newInitiativeCancelled(){
        this.newInitiative=false;
    }
    newInitiativeSubmitted(){
        this.newInitiative=false;
        this.getList();
    }
}
