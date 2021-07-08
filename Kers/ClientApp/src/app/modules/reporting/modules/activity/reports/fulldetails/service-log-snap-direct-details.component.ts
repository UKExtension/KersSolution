import { Component, OnInit, Input } from '@angular/core';
import { SnapDirect, ServicelogService, SnapDirectSessionType, SnapDirectDeliverySite, SnapDirectAges, SnapDirectAudience, SnapDirectAgesAudienceValue } from '../../../servicelog/servicelog.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'service-log-snap-direct-details',
  templateUrl: './service-log-snap-direct-details.component.html'
})
export class ServiceLogSnapDirectDetailsComponent implements OnInit {
  @Input() snapDirectId;
  snapDirect:Observable<SnapDirect>|null = null;
  ages:Observable<SnapDirectAges[]>;
  audiences:Observable<SnapDirectAudience[]>;
  loading=false;

  constructor(
    private service:ServicelogService
  ) { 
    // Snap Direct
    this.audiences = this.service.snapdirectaudience();
    this.ages = this.service.snapdirectages();
  }

  ngOnInit() {
    this.snapDirect = this.service.getSnapDirect(this.snapDirectId);
  }

  getAgeAudienceValue(snapDirectAgesAudienceValues:SnapDirectAgesAudienceValue[], ageid, audienceid){
    var val = snapDirectAgesAudienceValues.filter( v => v.snapDirectAgesId == ageid && v.snapDirectAudienceId == audienceid);
    return val.length == 0 ? 0 : val[0].value;
  }
  getTotalPerAudience(snapDirectAgesAudienceValues:SnapDirectAgesAudienceValue[], audienceid){
    var val = snapDirectAgesAudienceValues.filter( v => v.snapDirectAudienceId == audienceid);
    var sum = 0;
    for( var s of val){
      sum += s.value;
    }
    return sum;
  }
  getTotal(snapDirectAgesAudienceValues:SnapDirectAgesAudienceValue[]){
    var sum = 0;
    for( var s of snapDirectAgesAudienceValues){
      sum += s.value;
    }
    return sum;
  }


}
