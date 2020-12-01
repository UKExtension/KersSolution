import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ProgramCategory } from '../../admin/programs/programs.service';
import { Mileage } from '../../mileage/mileage';
import { MileageService } from '../../mileage/mileage.service';
import { ExpenseFundingSource } from '../expense.service';

@Component({
  selector: 'mileage-reports-details-item',
  template: `
  <loading *ngIf="loading"></loading>
  <div class="col-md-12 col-sm-12 col-xs-12" *ngIf="!loading">
    <div class="ln_solid"></div>
        <div class="row">
            <div class="col-sm-4">
                <p *ngIf="expense.vehicleType == 2">County Vehicle</p>
                <p *ngIf="expense.vehicleType != 2">Personal Vehicle</p>
                <h3 style="margin-bottom:0;">{{expense.expenseDate| date:'mediumDate'}}</h3>
                <p *ngIf="expense.isOvernight">Overnight Trip</p>
                <p *ngIf="!expense.isOvernight">Day Trip</p>
            </div>
            <div class="col-sm-8">
                <p *ngIf="expense.vehicleType == 2">{{countyVehicle}}</p>
                <p *ngIf="expense.comment != null && expense.comment!=''"><strong>Comment: </strong>{{expense.comment}}</p>
            </div>
          </div>
          <div class="row">
            <div class="col-sm-4">
              <h4>Starting Location:</h4>
            </div>
            <div class="col-sm-8">
              <h5>{{expense.startingLocation.address.building}}<strong *ngIf="expense.startingLocation.displayName != null && expense.startingLocation.displayName != ''"> ({{expense.startingLocation.displayName}})</strong></h5>
              <h5>{{expense.startingLocation.address.street}}</h5>
              <h5>{{expense.startingLocation.address.city}} {{expense.startingLocation.address.state != ""?", "+expense.startingLocation.address.state:""}} {{expense.startingLocation.address.postalCode}}</h5>
              <br><br>
            </div>
          </div>
          <div class="row">
            <div class="col-sm-4">
              <h4>Destinations:</h4>
            </div>
            <div class="col-sm-8">
              <div *ngFor="let segment of expense.segments">
                <br>
                <h5>{{segment.location.address.building}}<strong *ngIf="segment.location.displayName != null && segment.location.displayName != ''"> ({{segment.location.displayName}})</strong></h5>
                <h5>{{segment.location.address.street}}</h5>
                <h5>{{segment.location.address.city}} {{segment.location.address.state != ""?", "+segment.location.address.state:""}} {{segment.location.address.postalCode}}</h5>
                <br>
                <p><strong>Business Purpose:</strong> {{segment.businessPurpose}}</p>
                <p><strong>Funding Source:</strong> {{source(segment.fundingSourceId)}}</p>
                <p><strong>Program Category:</strong> {{category(segment.programCategoryId)}}</p>
                <p><strong>Miles:</strong> {{segment.mileage}}</p>
                <div class="ln_solid"></div>
              </div>
        </div>
  </div>
  `,
  styles: []
})
export class MileageReportsDetailsItemComponent implements OnInit {
  @Input() expense:Mileage;
  @Input() categories:ProgramCategory[];
  @Input() sources:ExpenseFundingSource[];
  loading = true;
  countyVehicle:string = "";

  constructor(
    private mileageService:MileageService
  ) { }

  ngOnInit() {
    this.mileageService.byRevId(this.expense.id).subscribe(
      res => {
        this.expense = res;
        this.loading = false;
        if(this.expense.vehicleType == 2){
          this.mileageService.vehicle(this.expense.countyVehicleId).subscribe(
            res => this.countyVehicle = res.name + ", " + res.make + " " + res.model
          )
        }
      }
    )
  }

  source( id:number ):string{
    var name = "";
    var cat = this.sources.filter(s => s.id == id);
    if( cat.length > 0) name = cat[0].name;
    return name;
  }

  category( id:number ):string{
    var name = "";
    var cat = this.categories.filter(s => s.id == id);
    if( cat.length > 0) name = cat[0].name;
    return name;
  }

}
