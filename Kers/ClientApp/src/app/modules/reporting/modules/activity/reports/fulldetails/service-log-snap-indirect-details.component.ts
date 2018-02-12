import { Component, OnInit, Input } from '@angular/core';
import { ServicelogService, SnapIndirect, SnapIndirectReached, SnapIndirectReachedValue } from '../../../servicelog/servicelog.service';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'service-log-snap-indirect-details',
  templateUrl: './service-log-snap-indirect-details.component.html'
})
export class ServiceLogSnapIndirectDetailsComponent implements OnInit {
  @Input() snapIndirectId;
  snapInDirect:Observable<SnapIndirect>|null = null;
  reached:Observable<SnapIndirectReached[]>|null = null;
  loading=false;

  constructor(
    private service:ServicelogService
  ) { 
    this.reached = service.snapindirectreached();
  }

  ngOnInit() {
    this.snapInDirect = this.service.getSnapInDirect(this.snapIndirectId);
  }
  getReachedValue(vals:SnapIndirectReachedValue[], id){
    var val = vals.filter( v => v.snapIndirectReachedId == id);
    return val.length == 0 ? 0 : val[0].value;
  }
}
