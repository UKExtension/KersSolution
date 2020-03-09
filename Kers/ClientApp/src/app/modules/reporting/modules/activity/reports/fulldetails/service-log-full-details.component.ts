import { Component, OnInit, Input } from '@angular/core';
import { Activity, ActivityOption, Race, Ethnicity, ActivityOptionNumber, ActivityService } from '../../activity.service';
import { Observable } from 'rxjs/Observable';
import { ServicelogService } from '../../../servicelog/servicelog.service';

@Component({
  selector: 'service-log-full-details',
  templateUrl: './service-log-full-details.component.html'
})
export class ServiceLogFullDetailsComponent implements OnInit {
  @Input() activity:Activity;
  @Input() activityId:number;
  races:Observable<Race[]>;
  ethnicities:Observable<Ethnicity[]>;
  loading = true;

  constructor(
    private service: ActivityService,
    private serviceLogService: ServicelogService
  ) {
    this.races = this.service.races();
    this.ethnicities = this.service.ethnicities();
   }

  ngOnInit() {
    if(this.activity == null){
      if(this.activityId != null){
        this.serviceLogService.byId(this.activityId).subscribe(
          res => {
            this.activity = res;
            this.loading = false;
          }
        )
        
      }
      
    }else{
      this.loading = false;
    }
  }

  getRaceEthnicityValue(raceId:number, ethnicityId:number){
    var val = this.activity.raceEthnicityValues.filter( v => v.ethnicityId == ethnicityId && v.raceId== raceId);
    return val.length == 0 ? 0 : val[0].amount;
  }
  getTotalPerRace(raceId:number){
    var val = this.activity.raceEthnicityValues.filter( v => v.raceId== raceId);
    var sum = 0;
    for( var s of val){
      sum += s.amount;
    }
    return sum;
  }
  getTotal(){
    var sum = 0;
    for( var s of this.activity.raceEthnicityValues){
      sum += s.amount;
    }
    return sum;
  }

}
