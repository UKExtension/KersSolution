import { Component, OnInit } from '@angular/core';



@Component({
    selector: 'widget-specialist',
    template: `
    <div class="col-md-6 col-xs-12">
        <div class="x_panel">
          <div class="x_title">
            <h2>Report Activities</h2>
            <div class="clearfix"></div>
          </div>
          <div class="x_content">
            <a routerLink="/reporting/contact" class="btn btn-dark btn-lg btn-block">Statistical Contacts</a>
            <a routerLink="/reporting/story" class="btn btn-dark btn-lg btn-block">Success Stories</a>
          </div>
        </div>
    </div>
  `
})
export class WidgetSpecialistComponent { 


}