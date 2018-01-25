import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs/Observable';
import {InitiativeFormComponent} from './initiative-form.component';
import {ProgramsService, StrategicInitiative, MajorProgram} from './programs.service';

@Component({
    template: `
<div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newInitiative" (click)="newInitiativeOpen()">+ new statigic initiative</a>
    </div>
    <initiative-form *ngIf="newInitiative" (onFormCancel)="newInitiativeCancelled()" (onFormSubmit)="newInitiativeSubmitted()"></initiative-form>
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
       
    `

})
export class InitiativeListComponent implements OnInit{


    errorMessage: string;
    newInitiative = false;

    initiatives: StrategicInitiative[];

    constructor(
        private router: Router,
        private service: ProgramsService
    ){}

    ngOnInit(){
        this.getList();
    }

    getList(){
        
        this.service.listInitiatives().subscribe(
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
