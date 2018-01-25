import { Component, Input, Output, EventEmitter } from '@angular/core';
import {HelpService, HelpCategory} from './help.service';


@Component({
  selector: '[help-category-detail]',
  template: `
  <div class="block">
    <div class="tags">
        <strong>
        {{category.title}}
        </strong>
    </div>
    <div class="block_content">
        <h2 class="title">
            <div *ngIf="row">
                <a class="btn btn-info btn-xs" (click)="openChildren()">sub categories</a>
                <a class="btn btn-info btn-xs" (click)="onEdit()">edit</a>
                <a class="btn btn-info btn-xs" (click)="onDelete()">delete</a>
            </div>
            <div *ngIf="!row">
                <a class="btn btn-info btn-xs" (click)="close()">close</a>
            </div>
        </h2>
        <help-category-form *ngIf="edit" [help]="category" (onFormCancel)="close()" (onFormSubmit)="updated($event)"></help-category-form>
        <help-category-list [parentId]="category.id" *ngIf="children"></help-category-list>
        <div *ngIf="delete">Do you really want to delete the help category {{category.title}}?<br><button (click)="confirmDelete()" class="btn btn-info btn-xs">Yes</button> <button (click)="close()" class="btn btn-info btn-xs">No</button></div>
        <p class="excerpt" *ngIf="row">{{category.description}}</p>     
    </div>
</div>
    
  `
})
export class HelpCategoryDetailComponent { 

    @Input('help-category-detail') category:HelpCategory;

    row = true;
    children = false;
    edit = false;
    delete = false;

    @Output() onHelpCategoryDeleted = new EventEmitter<HelpCategory>();
    errorMessage: string;

    constructor( 
        private helpService:HelpService
    )   
    {}

    ngOnInit(){
        
    }

    openChildren(){
        this.row = false;
        this.children = true;
    }

    onEdit(){
        this.row = false;
        this.edit = true;
    }

    updated(cat:HelpCategory){
        this.row=true;
        this.edit = false;
        this.category = cat;
    }

    onDelete(){
        this.row = false;
        this.delete = true;
    }

    confirmDelete(){
        this.helpService.deleteHelpCategory(this.category.id).subscribe(
            res => {
                    this.onHelpCategoryDeleted.emit(this.category);
                    return res;
                },
            error =>  this.errorMessage = <any>error
        );
    }

    close(){
        this.row=true;
        this.children = false;
        this.edit = false;
        this.delete = false;
    }

    
}