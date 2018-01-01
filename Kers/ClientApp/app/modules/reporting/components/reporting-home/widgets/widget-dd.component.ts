import { Component, OnInit } from '@angular/core';



@Component({
    selector: 'widget-dd',
    template: `
    <div class="col-md-6 col-xs-12">
        <div class="x_panel">
          <div class="x_title">
            <h2>Reports</h2>
            <div class="clearfix"></div>
          </div>
          <div class="x_content">
            <a routerLink="/reporting/state/district/plans" class="btn btn-dark btn-lg btn-block">Plans of Work</a>
            <a routerLink="/reporting/state/district" class="btn btn-dark btn-lg btn-block">Employee Summaries</a>
          </div>
        </div>
    </div>
  `
})
export class WidgetDDComponent { 


}