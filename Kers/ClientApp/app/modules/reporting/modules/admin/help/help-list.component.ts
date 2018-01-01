import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import { Observable } from 'rxjs/Observable';
import {HelpFormComponent} from './help-form.component';
import {HelpService, Help, HelpCategory} from './help.service';

@Component({
    template: `{{errorMessage}}
<div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newHelp" (click)="newHelpOpen()">+ new help content</a>
    </div>
    <help-form *ngIf="newHelp" (onFormCancel)="newHelpCancelled()" (onFormSubmit)="newHelpSubmitted($event)"></help-form>
</div>
<div *ngIf="helps">
    <table class="table table-striped">
        <thead>
        <tr>
            <th>Id</th>
            <th>Title</th>
            <th></th>
        </tr>
        </thead>
        <tbody>
            <tr *ngFor="let help of helps" [helpListDetail]="help" (onHelpUpdated)="onHelpUpdate()" (onHelpDeleted)="onHelpDeleted($event)"></tr>
        </tbody>               
    </table>            
</div>
       
    `

})
export class HelpListComponent implements OnInit{


    errorMessage: string;
    newHelp = false;

    helps: Help[];
    categories: HelpCategory[];

    constructor(
        private router: Router,
        private service: HelpService
    ){}

    ngOnInit(){
        this.getList();
        this.service.allCategories().subscribe(
            categories => this.categories = categories,
            error =>  this.errorMessage = <any>error
        );
    }

    getList(){
        
        this.service.all().subscribe(
            helps => this.helps = helps,
            error =>  this.errorMessage = <any>error
        );
        
    }

    onHelpUpdate(){
        //this.getList();
    }

    onHelpDeleted(help:Help){
        let index: number = this.helps.indexOf(help);
        if (index !== -1) {
            this.helps.splice(index, 1);
        }
    }
    newHelpOpen(){
        this.newHelp = true;
    }

    newHelpCancelled(){
        this.newHelp=false;
    }
    newHelpSubmitted(event:Help){
        this.newHelp=false;
        this.helps.push(event)
    }
}
