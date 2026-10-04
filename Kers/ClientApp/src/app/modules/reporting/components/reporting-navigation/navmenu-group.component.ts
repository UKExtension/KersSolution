import {
  Component,
  EventEmitter,
  Input,
  Output,
  ChangeDetectionStrategy
} from '@angular/core';
import { NavGroup, NavItem} from './navigation.service';

@Component({
    selector: '[nav-menu-group]',
    template: `
        <a (click)="toggleOpen($event)"><i class="fa {{sectionGroup.icon}}"></i> {{sectionGroup.name}} <span class="fa fa-chevron-down"></span></a>
                <div class="child_menu_wrapper" [class.open]="sectionGroup.isOpen === 'active'">
                    <ul class="nav child_menu" style="display: block">
                        @for (item of sectionGroup.items; track item) {
                            <li routerLinkActive="active" [nav-menu-item]="item" (onSelected)="itemSelected($event)">
                            </li>
                        }
                    </ul>
                </div>
        
        `,
    styles: [
        `
        .child_menu_wrapper {
            display: grid;
            grid-template-rows: 0fr;
            visibility: hidden;
            transition: grid-template-rows 250ms cubic-bezier(0.1, 0.1, 0.2, 0.9), visibility 0s linear 250ms;
        }
        .child_menu_wrapper.open {
            grid-template-rows: 1fr;
            visibility: visible;
            transition: grid-template-rows 250ms cubic-bezier(0.1, 0.1, 0.2, 0.9), visibility 0s;
        }
        .child_menu {
            display: block;
            min-height: 0;
            overflow: hidden;
        }
        `
    ],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class NavmenuGroupComponent {
    @Input('nav-menu-group') sectionGroup: NavGroup;
    @Output() onOpen = new EventEmitter<NavGroup>();
    @Output() onSelected = new EventEmitter<NavItem>();

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

    itemSelected(event){
        this.onSelected.emit(event);
    }
}