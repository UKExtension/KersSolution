import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import {NavItem} from './navigation.service';

@Component({
    selector: '[nav-menu-item]',
    template: `
   
        <a *ngIf="itemData.isRelative" (click)="clicked($event)" routerLink="{{itemData.route}}" routerLinkActive="active" [routerLinkActiveOptions]="{exact:
true}" href="{{itemData.route}}" class="nav-item">{{itemData.name}}</a>
        <a *ngIf="!itemData.isRelative" (click)="clicked($event)" href="{{itemData.route}}" class="nav-item">{{itemData.name}}</a>

    `,
    styleUrls: ['./navmenu-item.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class NavmenuItemComponent {
    @Input('nav-menu-item') itemData: NavItem;
    @Output() onSelected = new EventEmitter<NavItem>();
    
    clicked(event){
        this.onSelected.emit(this.itemData);
    }
}