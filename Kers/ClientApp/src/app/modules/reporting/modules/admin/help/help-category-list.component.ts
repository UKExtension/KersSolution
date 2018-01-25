import { Component, Input } from '@angular/core';
import {HelpService, HelpCategory} from './help.service';


@Component({
  selector: 'help-category-list',
  template: `
  <div class="row">
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newCategory && parentId==0" (click)="newCategoryOpen()">+ new category</a>
        <a class="btn btn-info btn-xs" *ngIf="!newCategory && parentId!=0" (click)="newCategoryOpen()">+ new sub category</a>
    </div>
    <help-category-form *ngIf="newCategory" [parentId]="parentId" (onFormCancel)="newCategoryClose()" (onFormSubmit)="newCategorySubmit($event)" ></help-category-form>
    <div *ngIf="categories">
        <ul class="list-unstyled timeline">
            <li *ngFor="let category of categories" [help-category-detail]="category" (onHelpCategoryDeleted)="categoryDeleted($event)"></li>           
        </ul>
    </div>
  </div>
  `
})
export class HelpCategoryListComponent { 

    @Input('parentId') parentId:number;
    errorMessage: string;
    categories: HelpCategory[];
    newCategory = false;

    constructor( 
        private helpService: HelpService 
    )   
    {}

    ngOnInit(){
        
        this.helpService.categoryChildren(this.parentId).subscribe(
            cats => this.categories = cats,
            error =>  this.errorMessage = <any>error
        );

    }

    newCategorySubmit(cat:HelpCategory){
        this.categories.push(cat);
        this.newCategory = false;
    }

    newCategoryOpen(){
        this.newCategory = true;
    }

    newCategoryClose(){
        this.newCategory = false;
    }
    categoryDeleted(cat){
        let index: number = this.categories.indexOf(cat);
        if (index !== -1) {
            this.categories.splice(index, 1);
        }
    }
}