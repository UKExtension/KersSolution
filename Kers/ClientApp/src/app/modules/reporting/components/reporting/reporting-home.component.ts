import { Component } from '@angular/core';
import {ReportingService} from './reporting.service';

@Component({
  template: `
    <div [innerHtml]="stats.name"></div>
    <div class="x_panel">
      <div class="x_title">
        <h2>{{title.name}}<small [hidden]="!subtitle || subtitle.name == ''">{{subtitle.name}}</small></h2>
        <div class="clearfix"></div>
      </div>
      <div class="x_content">
          <router-outlet></router-outlet>
      </div>
    </div>
    
  `
})
export class ReportingHomeComponent { 
  public title:any;
  public subtitle:any;
  public stats:any;

  constructor( 
        private reportingService: ReportingService
        ) 
    {
        
        
        
    }
    ngOnInit(){
      this.stats = this.reportingService.statsHtml;
      this.title = this.reportingService.title;
      this.subtitle = this.reportingService.subtitle;
    }
}