import { Component, Input } from '@angular/core';
import {NavItem} from './navigation.service';

@Component({
    selector: '[nav-menu-item]',
    template: `
   
        <a *ngIf="itemData.isRelative" routerLink="{{itemData.route}}" routerLinkActive="active" [routerLinkActiveOptions]="{exact:
true}" href="{{itemData.route}}" class="nav-item">{{itemData.name}}</a>
        <a *ngIf="!itemData.isRelative" href="{{itemData.route}}" class="nav-item">{{itemData.name}}</a>

    `,
    styleUrls: ['./navmenu-item.component.css']
})
export class NavmenuItemComponent {
    @Input('nav-menu-item') itemData: NavItem;
}