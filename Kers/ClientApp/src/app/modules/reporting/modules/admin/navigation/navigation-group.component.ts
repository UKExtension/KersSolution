import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';


import { NavigationService, NavSection, NavGroup } from '../../../components/reporting-navigation/navigation.service';
import {AdminNavigationService} from './admin-navigation.service';

@Component({
    selector: 'admin-nav-groups',
    template: `
  <div>
    <div class="text-right">
      @if (!newGroup) {
        <a class="btn btn-info btn-xs" (click)="newGroup = true">+ new group</a>
      }
    </div>
    @if (newGroup) {
      <div>
        <navigation-group-form [section]="section" (onFormSubmit)="groupAdded($event)" (onFormCancel)="onCancel()"></navigation-group-form>
      </div>
    }
  </div>
  @if (groups) {
    <div>
      <table class="table table-striped">
        <tbody>
          @for (group of groups; track group) {
            <tr [navigationGroupDetail]="group" (onGroupDeleted)="groupDeleted($event)" (onGroupUpdated)="groupUpdated($event)" ></tr>
          }
        </tbody>
      </table>
    </div>
  }
  
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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