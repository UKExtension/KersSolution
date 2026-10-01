import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';

import { NavigationService, NavItem, NavGroup } from '../../../components/reporting-navigation/navigation.service';
import {AdminNavigationService} from './admin-navigation.service';

@Component({
    selector: 'admin-nav-items',
    template: `
  <div>
    <div class="text-right">
      @if (!newItem) {
        <a class="btn btn-info btn-xs" (click)="newItem = true">+ new item</a>
      }
    </div>
    @if (newItem) {
      <div>
        <navigation-item-form [group]="group" (onFormSubmit)="onNewItem($event)" (onFormCancel)="onCancel()"></navigation-item-form>
      </div>
    }
  </div>
  @if (items) {
    <div>
      <table class="table table-striped">
        @for (item of items; track item) {
          <tr [navigationItemDetail]="item" (onItemDeleted)="onItemDeleted($event)" (onItemUpdated)="itemUpdated($event)" ></tr>
        }
      </table>
    </div>
  }
  
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class NavigationItemsComponent { 

    errorMessage: string;
    @Input() group:NavGroup;
    items: NavItem[];


    @Output() itemAdded = new EventEmitter<NavItem>();
    @Output() itemDeleted = new EventEmitter<NavItem>();  

    newItem = false;

    constructor( 
        private navigationService: NavigationService,
        private adminNavigationService: AdminNavigationService 
    )   
    {
        
    }

    ngOnInit(){
        this.items = this.group.items;
    }

    groupUpdated(){

    }

    onCancel(){
        this.newItem = false;
    }

    onNewItem(event:NavItem){
        this.itemAdded.emit(event);
        this.newItem = false;
    }

    itemUpdated(){
        
    }

    onItemDeleted(event:NavItem){
        this.itemDeleted.emit(event);
    }
    


}