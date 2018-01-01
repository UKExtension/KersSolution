import { Component } from '@angular/core';
import {ReportingService} from '../../../components/reporting/reporting.service';


@Component({
  template: `
  
   <help-category-list [parentId]="0"><help-category-list>
    
  `
})
export class HelpCategoryComponent { 

    errorMessage: string;
    newCategory = false;

    constructor( 
        private reportingService: ReportingService,
    )   
    {}

    ngOnInit(){
        
        this.defaultTitle();
    }

    defaultTitle(){
        this.reportingService.setTitle("Help Category Management");
    }

}