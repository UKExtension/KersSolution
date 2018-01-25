import { Component, Input, Output, EventEmitter } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';


import { NavigationService, NavSection, NavGroup } from '../../../components/reporting-navigation/navigation.service';
import {AdminNavigationService} from './admin-navigation.service';

@Component({
    selector: 'admin-nav-groups',
  template: `
  <div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newGroup" (click)="newGroup = true">+ new group</a>
    </div>
 <div *ngIf="newGroup">
    <navigation-group-form [section]="section" (onFormSubmit)="groupAdded($event)" (onFormCancel)="onCancel()"></navigation-group-form>
 </div>
 </div>
    <div *ngIf="groups">
        <table class="table table-striped">
            <tr *ngFor="let group of groups" [navigationGroupDetail]="group" (onGroupDeleted)="groupDeleted($event)" (onGroupUpdated)="groupUpdated($event)" ></tr>
        </table>
    </div>

  `
})
export class NavigationGroupComponent { 

    errorMessage: string;
    @Input() section:NavSection;
    groups: NavGroup[];
    @Output() onGroupAdded = new EventEmitter<NavGroup>();


    newGroup = false;

    constructor( 
        private reportingService: ReportingService,
        private navigationService: NavigationService,
        private adminNavigationService: AdminNavigationService
    )   
    {
       
    }

    ngOnInit(){
        this.groups = this.section.groups;
    }

    groupUpdated(group){

    }

    groupDeleted(group){
        var index = this.groups.indexOf(group);
        if (index >= 0) {
            this.groups.splice( index, 1 );
        }
    }

    onCancel(){
        this.newGroup = false;
    }

    groupAdded(event){
        this.onGroupAdded.emit(<NavGroup>event);
        this.newGroup = false;
    }



}