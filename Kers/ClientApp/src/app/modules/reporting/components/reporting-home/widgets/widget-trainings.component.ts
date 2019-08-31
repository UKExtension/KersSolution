import { Component, OnInit } from '@angular/core';



@Component({
    selector: 'widget-trainings',
    template: `
    <div class="col-md-6 col-xs-12">
        <div class="x_panel">
          <div class="x_title">
            <h2>Trainings</h2>
            <div class="clearfix"></div>
          </div>
          <div class="x_content">
            <a routerLink="/reporting/training/catalog" class="btn btn-dark btn-lg btn-block">In-Service Trainings</a>
          </div>
        </div>
    </div>
  `
})
export class WidgetTrainingsComponent { 


}