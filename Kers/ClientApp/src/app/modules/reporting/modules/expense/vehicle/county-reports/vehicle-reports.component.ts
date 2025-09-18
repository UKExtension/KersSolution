import { Component, Input, OnInit } from '@angular/core';
import { TripsSearchCriteria, Vehicle, VehicleService } from '../vehicle.service';
import { Expense } from '../../expense.service';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { MileageBundle } from '../../../mileage/mileage';
import { Observable, Subject } from 'rxjs';
import { flatMap, startWith } from 'rxjs/operators';

@Component({
  selector: 'vehicle-reports',
  template: `
    <article class="media event">
              <div class="media-body">
              <a class="title" [ngStyle]="{ 'color' : (vehicle.enabled)? 'rgb(35, 82, 124);' : '#ccc' }">{{vehicle.year}} {{vehicle.make}}<span *ngIf="vehicle.name != undefined && vehicle.name != ''"> ({{vehicle.name}})</span></a>
              <p [ngStyle]="{ 'color' : (vehicle.enabled)? 'rgb(115, 135, 156)' : '#ccc' }">{{vehicle.model}}</p>
              </div>
     </article>
     <article *ngIf="trips$ != null">
     <h4 style="padding-top:14px;">Trips</h4>


     <div class="col-sm-12 col-xs-12 text-right" style="margin-top: -33px;">
      <div class="input-group" style="width:250px; float:right;">   
        <input type="text" class="form-control input-box" placeholder="Click to select a date" 
        angular-mydatepicker name="mydate" (click)="dp.toggleCalendar()" 
        [(ngModel)]="model" [options]="myDateRangePickerOptions" 
        #dp="angular-mydatepicker" (dateChanged)="dateCnanged($event)">
        <span class="input-group-addon" id="basic-addon1" (click)="dp.toggleCalendar()" style="cursor: pointer;"><i class="fa fa-calendar"></i></span>
      </div>
    </div>


     <div *ngFor="let trip of trips$ | async">
      <county-vehicle-trip [expense]="trip"></county-vehicle-trip>
     </div>
     </article>


    




  `,
  styles: [
    
  ]
})
export class VehicleReportsComponent implements OnInit {
  @Input() vehicle:Vehicle;
  trips$:Observable<MileageBundle[]>;

  refresh: Subject<string>; // For load/reload
  model: IMyDateModel = null;
  myDateRangePickerOptions: IAngularMyDpOptions = {
      dateRange: true,
      dateFormat: 'mmm dd, yyyy'
  };
  startDate:Date;
  endDate:Date;
  criteria:TripsSearchCriteria;

  constructor(
    private service:VehicleService
  ) { }

  ngOnInit(): void {
    this.startDate = new Date();
    this.startDate.setMonth( this.startDate.getMonth() - 5);
    this.endDate = new Date();
    

    let begin: Date = this.startDate;
    let end: Date = this.endDate;
    this.model = {
        isRange: true, 
        singleDate: null, 
        dateRange: {
          beginDate: {
            year: begin.getFullYear(), month: begin.getMonth() + 1, day: begin.getDate()
          },
          endDate: {
            year: end.getFullYear(), month: end.getMonth() + 1, day: end.getDate()
          }
        }
      };
      this.criteria = {
                start: this.startDate.toISOString(),
                end: this.endDate.toISOString()
      }

      this.refresh = new Subject();
          
      this.trips$ = this.refresh.asObservable()
            .pipe(
              startWith('onInit'), // Emit value to force load on page load; actual value does not matter
              flatMap(_ => this.service.trips(this.vehicle,this.criteria))
            );

      /*  this.service.trips(this.vehicle).subscribe(
          res => this.trips = res
        ); */
    }
    dateCnanged(event: IMyDateModel){

      this.startDate = event.dateRange.beginJsDate;
      this.endDate = event.dateRange.endJsDate;
      this.criteria["start"] = this.startDate.toISOString();
      this.criteria["end"] = this.endDate.toISOString();
      this.onRefresh();
    }
    onRefresh() {
      this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
    }

}


