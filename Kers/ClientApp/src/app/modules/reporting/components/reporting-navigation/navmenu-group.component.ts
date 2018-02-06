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
    selector: '[nav-menu-group]',
    template: `
        <a (click)="toggleOpen($event)"><i class="fa {{sectionGroup.icon}}"></i> {{sectionGroup.name}} <span class="fa fa-chevron-down"></span></a>
        <ul class="nav child_menu" [@groupState]="sectionGroup.isOpen" [style.display]="sectionGroup.isOpen?'block':'none'">
            <li routerLinkActive="active" *ngFor = "let item of sectionGroup.items" [nav-menu-item]="item">
            </li>
        </ul>
    
    `,
    styles: [
        `
    .child_menu{
      display:block;
      overflow:hidden;
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
    @Input('nav-menu-group') sectionGroup: NavGroup;
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