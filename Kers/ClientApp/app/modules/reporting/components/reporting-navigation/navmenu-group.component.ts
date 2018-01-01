import {    Component, 
            EventEmitter, 
            Input, 
            Output,
            trigger,
            state,
            style,
            transition,
            animate 
        } from '@angular/core';
import { NavGroup, NavItem} from './navigation.service';

@Component({
    selector: 'nav-menu-group',
    template: `
    <li class="nav-group" *ngIf="sectionGroup.items.length > 0" [class.active]="this.sectionGroup.isOpen == 'active'"><a (click)="toggleOpen($event)"><i class="fa {{sectionGroup.icon}}"></i> {{sectionGroup.name}} <span class="fa fa-chevron-down"></span></a>
        <ul class="nav child_menu" [@groupState]="this.sectionGroup.isOpen">
            <nav-menu-item *ngFor = "let item of sectionGroup.items" [item]="item"></nav-menu-item>
        </ul>
    </li>
    `,
    styles: [
        `
  
.nav-group {
    position: relative;
    display: block;
    cursor: pointer; 
}
li.active {
    border-right: 5px solid #1ABB9C;
}
.nav-group.active a{
    text-shadow: rgba(0, 0, 0, 0.25) 0 -1px 0;
    background: linear-gradient(#334556, #2C4257), #2A3F54;
    box-shadow: rgba(0, 0, 0, 0.25) 0 1px 0, inset rgba(255, 255, 255, 0.16) 0 1px 0    
}
.nav-group > a {
    color: #E7E7E7;
    font-weight: 500; 
    position: relative;
    display: block;
    padding: 13px 15px 12px
}
.child_menu{
    max-height: 300px;
    overflow: hidden;
}
        `
    ],
    animations: [
        trigger('groupState', [
            state('inactive', style({
                height: 0
            })),
            state('active',   style({
                height: "*"
            })),
            transition("inactive <=> active", animate('250ms cubic-bezier(0.1, 0.1, 0.2, 0.9)'))
        ])
    ]
})
export class NavmenuGroupComponent {
    @Input('group') sectionGroup: NavGroup;
    @Output() onOpen = new EventEmitter<NavGroup>();

    construct(){
        if(this.sectionGroup.isOpen == 'active'){
            this.open(new Event(this.sectionGroup.name));
        }
    }

    toggleOpen(event){
        if(this.sectionGroup.isOpen == 'active'){
            this.close(event);
        }else{
            this.open(event);
        }
    }

    open(event){
        this.sectionGroup.isOpen = 'active';
        this.onOpen.emit(this.sectionGroup);
    }

    close(event){
        this.sectionGroup.isOpen = 'inactive';
    }
}