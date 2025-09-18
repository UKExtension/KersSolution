import { Component, Input, OnInit } from '@angular/core';
import { PlanningUnit } from '../../../user/user.service';

@Component({
  selector: 'county-vehicles-reports',
  template: `
    <div class="row" *ngIf="county && county.vehicles && county.vehicles.length > 0">
        <div class="col-md-12">
            <div class="x_panel">
            <div class="x_title" style="border-bottom:none; margin-bottom:-20px;">
                <h2>County Vehicles</h2>

                <div class="text-right" style="padding: 10px 0;">
                  <span style="vertical-align:top;">Include Former Vehicles &nbsp;</span> 
                  <label class="switch">
                      <input type="checkbox" id="onlyEnabledCheckbox">
                      <div class="slider round" (click)="includeLeftChecked()"></div>
                  </label>
              
                </div>

                <div class="clearfix"></div>
            </div>
            <div class="x_content">
                <div *ngIf="county">
                  <div *ngFor="let vehicle of county.vehicles">
                    <ng-container *ngIf="vehicle.enabled || inludeFormer">
                      <vehicle-list-detail [vehicle]="vehicle" [trips]="true"></vehicle-list-detail>
                    </ng-container>
                  </div>
                  
                </div> 
            </div>
        </div>
    </div>
  `,
  styles: [
    `
        .switch {
          position: relative;
          display: inline-block;
          width: 30px;
          height: 17px;
        }
        
        /* Hide default HTML checkbox */
        .switch input {display:none;}
        
        /* The slider */
        .slider {
          position: absolute;
          cursor: pointer;
          top: 0;
          left: 0;
          right: 0;
          bottom: 0;
          background-color: #ccc;
          -webkit-transition: .4s;
          transition: .4s;
        }
        
        .slider:before {
          position: absolute;
          content: "";
          height: 13px;
          width: 13px;
          left: 2px;
          bottom: 2px;
          background-color: white;
          -webkit-transition: .4s;
          transition: .4s;
        }
        
        input:checked + .slider {
          background-color: rgb(38, 185, 154);
          border-color: rgb(38, 185, 154); 
          box-shadow: rgb(38, 185, 154) 
        }
        
        input:focus + .slider {
          box-shadow: 0 0 1px rgb(38, 185, 154);
        }
        
        input:checked + .slider:before {
          -webkit-transform: translateX(13px);
          -ms-transform: translateX(13px);
          transform: translateX(13px);
        }
        
        /* Rounded sliders */
        .slider.round {
          border-radius: 17px;
        }
        
        .slider.round:before {
          border-radius: 50%;
        }
        
        `
  ]
})
export class CountyVehiclesReportComponent implements OnInit {

  @Input() county:PlanningUnit;
  inludeFormer:boolean = false;
  constructor(
    
  ) { }

  ngOnInit(): void {
  }

  includeLeftChecked(){
    this.inludeFormer = !this.inludeFormer;
  }

}
