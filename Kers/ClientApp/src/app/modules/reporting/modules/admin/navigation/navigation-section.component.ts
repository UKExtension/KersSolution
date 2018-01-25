import { Component } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';

import { NavigationService, NavSection } from '../../../components/reporting-navigation/navigation.service';
import {AdminNavigationService} from './admin-navigation.service';

@Component({
  template: `
  <div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newSection" (click)="newSection = true">+ new section</a>
    </div>
 <div *ngIf="newSection">
    <navigation-section-form (onFormCancel)="onCancel()" (onFormSubmit)="onNewSection()"></navigation-section-form>
 </div>
 </div>
    <div *ngIf="sections">
        <table class="table table-striped">
            <tr *ngFor="let section of sections" [navigationSectionDetail]="section" (onSectionDeleted)="sectionUpdated()" (onSectionUpdated)="sectionUpdated()" ></tr>
        </table>
    </div>

  `
})
export class NavigationSectionComponent { 

    errorMessage: string;
    sections: NavSection[];

    newSection = false;

    constructor( 
        private reportingService: ReportingService,
        private navigationService: NavigationService,
        private adminNavigationService: AdminNavigationService 
    )   
    {}

    ngOnInit(){
        
        this.loadSections();
        this.defaultTitle();
    }

    loadSections(){
        this.adminNavigationService.nav().subscribe(
            res => {
                this.sections = res;
            },
            error => this.errorMessage = <any> error
        );
    }

    onCancel(){
        this.newSection = false;
    }

    onNewSection(){
        this.newSection = false;
        this.loadSections();
    }

    sectionUpdated(){
        this.loadSections();
    }

    defaultTitle(){
        this.reportingService.setTitle("Navigation Section Management");
    }
}