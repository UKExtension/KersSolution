import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs/Observable';
import {InitiativeFormComponent} from './initiative-form.component';
import {ProgramsService, StrategicInitiative, MajorProgram} from './programs.service';
import { FiscalyearService, FiscalYear } from '../fiscalyear/fiscalyear.service';

@Component({
    template: `
    <div class="row" *ngIf="fiscalYears != null">
        <div class="col-xs-5">Fiscal Year: <span *ngFor="let year of fiscalYears"><a (click)="selectFiscalYear(year)" [class.active-year]="year.id == selectedFiscalYear.id" style="cursor:pointer;">{{year.name}}</a> | </span>
        </div>
    </div>
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

    fiscalYears: FiscalYear[];
    nextFiscalYear: FiscalYear;
    currentFiscalYear: FiscalYear;
    selectedFiscalYear: FiscalYear;

    constructor(
        private router: Router,
        private service: ProgramsService,
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
                this.getList();
            }
        );
    }
    selectFiscalYear(year:FiscalYear){
        if(this.selectedFiscalYear.id != year.id){
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
