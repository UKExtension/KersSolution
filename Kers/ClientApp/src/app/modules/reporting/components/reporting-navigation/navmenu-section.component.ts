import { Component, Input, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { NavSection, NavGroup, NavItem} from './navigation.service';

@Component({
    selector: 'nav-menu-section',
    template: `
    <div *ngIf="section.groups.length > 0" class="menu_section">
        <h3>{{section.name}}</h3>
        <ul class="nav side-menu">
            <li class="nav-group" *ngFor = "let group of section.groups" [class.active]="this.group.isOpen == 'active'" [nav-menu-group]="group" (onOpen)="closeOthers($event)"></li>
        </ul>
    </div>
    `
})
export class NavmenuSectionComponent implements OnInit{
    @Input('section') section: NavSection;


    constructor( 
        private route: ActivatedRoute, 
        private router: Router
        ) 
    {
        
        
    }

    closeOthers(event){
        for(var key in this.section.groups){
            if(this.section.groups[key].id != event.id){
                this.section.groups[key].isOpen = 'inactive';
            }
        }
    }

    ngOnInit() {
        for (var i = 0; i < this.section.groups.length; i++) {
            if(this.section.groups[i].items.length == 0){
                this.section.groups.splice(i, 1);
            }
        }
        this.openActive();
    }

    openActive(){
         for(var key in this.section.groups){
            for(var k in this.section.groups[key].items){
                if(this.section.groups[key].items[k].route === this.router.url){
                    this.section.groups[key].isOpen = 'active';
                    //return;
                }else{
                    this.section.groups[key].isOpen = 'inactive';
                }
            }
        }

    }
}