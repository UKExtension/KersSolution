import { Component, Input, ChangeDetectionStrategy } from '@angular/core';
import {HelpService, HelpCategory} from './help.service';


@Component({
    selector: 'help-category-list',
    template: `
  <div class="row">
    <div class="text-right">
      @if (!newCategory && parentId==0) {
        <a class="btn btn-info btn-xs" (click)="newCategoryOpen()">+ new category</a>
      }
      @if (!newCategory && parentId!=0) {
        <a class="btn btn-info btn-xs" (click)="newCategoryOpen()">+ new sub category</a>
      }
    </div>
    @if (newCategory) {
      <help-category-form [parentId]="parentId" (onFormCancel)="newCategoryClose()" (onFormSubmit)="newCategorySubmit($event)" ></help-category-form>
    }
    @if (categories) {
      <div>
        <ul class="list-unstyled timeline">
          @for (category of categories; track category) {
            <li [help-category-detail]="category" (onHelpCategoryDeleted)="categoryDeleted($event)"></li>
          }
        </ul>
      </div>
    }
  </div>
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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