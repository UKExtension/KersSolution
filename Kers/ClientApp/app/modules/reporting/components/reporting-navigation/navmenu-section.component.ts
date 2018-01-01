import { Component, Input, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { NavSection, NavGroup, NavItem} from './navigation.service';

@Component({
    selector: 'nav-menu-section',
    template: `
    <div *ngIf="section.groups.length > 0" class="menu_section">
        <h3>{{section.name}}</h3>
        <ul class="nav side-menu">
            <nav-menu-group *ngFor = "let group of section.groups" [group]="group" (onOpen)="closeOthers($event)"></nav-menu-group>
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