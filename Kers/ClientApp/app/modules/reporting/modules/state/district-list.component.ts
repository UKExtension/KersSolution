import { Component } from '@angular/core';
import {StateService} from './state.service';

import {Observable} from 'rxjs/Observable';
import { District } from "../district/district.service";

@Component({
  template: `
    <div class="col-md-12">
        <div class="x_panel">
            <div class="x_title">
                <h2>Districts</h2>
                <div class="clearfix"></div>
            </div>
            <div class="x_content">
                <div class="row">
                    <div class="col-lg-4 col-md-6 col-xs-12" *ngFor="let district of districts | async">
                        <a [routerLink]="['district', district.id]" class="btn btn-dark btn-lg btn-block">{{district.areaName}}<br><small>({{district.name}})</small></a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
  `
})
export class DistrictListComponent { 

    districts:Observable<District[]>;

    errorMessage:string;

    constructor( 
        private service:StateService
    )   
    {}

    ngOnInit(){
        this.districts = this.service.districts();
    }

}