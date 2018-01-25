import { Component, Input, Output, EventEmitter } from '@angular/core';

import { NavigationService, NavItem, NavGroup } from '../../../components/reporting-navigation/navigation.service';
import {AdminNavigationService} from './admin-navigation.service';

@Component({
    selector: 'admin-nav-items',
  template: `
  <div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newItem" (click)="newItem = true">+ new item</a>
    </div>
 <div *ngIf="newItem">
    <navigation-item-form [group]="group" (onFormSubmit)="onNewItem($event)" (onFormCancel)="onCancel()"></navigation-item-form>
 </div>
 </div>
    <div *ngIf="items">
        <table class="table table-striped">
            <tr *ngFor="let item of items" [navigationItemDetail]="item" (onItemDeleted)="onItemDeleted($event)" (onItemUpdated)="itemUpdated($event)" ></tr>
        </table>
    </div>

  `
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