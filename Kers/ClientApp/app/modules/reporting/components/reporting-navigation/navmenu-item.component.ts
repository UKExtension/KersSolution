import { Component, Input } from '@angular/core';
import {NavItem} from './navigation.service';

@Component({
    selector: 'nav-menu-item',
    template: `
    <li routerLinkActive="active" >
        <a *ngIf="itemData.isRelative" routerLink="{{itemData.route}}" routerLinkActive="active" [routerLinkActiveOptions]="{exact:
true}" href="{{itemData.route}}" class="nav-item">{{itemData.name}}</a>
        <a *ngIf="!itemData.isRelative" href="{{itemData.route}}" class="nav-item">{{itemData.name}}</a>

    </li>
    `,
    styleUrls: ['./navmenu-item.component.css']
})
export class NavmenuItemComponent {
    @Input('item') itemData: NavItem;
}