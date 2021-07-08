import {Component, OnInit} from '@angular/core';
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
        <a class="btn btn-info btn-xs" *ngIf="!newInitiative" (click)="newInitiativeOpen()">+ new statigic initiative</a>
    </div>
    <initiative-form *ngIf="newInitiative" (onFormCancel)="newInitiativeCancelled()" (onFormSubmit)="newInitiativeSubmitted()" [fiscalYear]="selectedFiscalYear"></initiative-form>
</div>
<div *ngIf="initiatives">
    <table class="table table-striped">
        <thead>
        <tr>
            <th>Name</th>
            <th>Category</th>
            <th></th>
        </tr>
        </thead>
        <tbody>
            <tr *ngFor="let initiative of initiatives" [initiativeListDetail]="initiative" (onInitiativeUpdated)="onInitiativeUpdate()" (onInitiativeDeleted)="onInitiativeUpdate()"></tr>
        </tbody>               
    </table>            
</div>
       
    `,
    styles: [`
        .active-year{
            font-weight: bold;
        }
    `]

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
        
        this.service.listInitiatives(this.selectedFiscalYear.name).subscribe(
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
