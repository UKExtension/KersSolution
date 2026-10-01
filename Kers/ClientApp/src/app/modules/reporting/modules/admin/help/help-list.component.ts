import {Component, OnInit, ChangeDetectionStrategy} from '@angular/core';
import {HelpService, Help, HelpCategory} from './help.service';

@Component({
    template: `{{errorMessage}}
<div>
  <div class="text-right">
    @if (!newHelp) {
      <a class="btn btn-info btn-xs" (click)="newHelpOpen()">+ new help content</a>
    }
  </div>
  @if (newHelp) {
    <help-form (onFormCancel)="newHelpCancelled()" (onFormSubmit)="newHelpSubmitted($event)"></help-form>
  }
</div>
@if (helps) {
  <div>
    <table class="table table-striped">
      <thead>
        <tr>
          <th>Id</th>
          <th>Title</th>
          <th></th>
        </tr>
      </thead>
      <tbody>
        @for (help of helps; track help) {
          <tr [helpListDetail]="help" (onHelpUpdated)="onHelpUpdate()" (onHelpDeleted)="onHelpDeleted($event)"></tr>
        }
      </tbody>
    </table>
  </div>
}

`,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class HelpListComponent implements OnInit{


    errorMessage: string;
    newHelp = false;

    helps: Help[];
    categories: HelpCategory[];

    constructor(
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
