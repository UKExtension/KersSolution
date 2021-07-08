import { Component, OnInit, Input } from '@angular/core';
import { ServicelogService, SnapPolicy, SnapPolicyPartner, SnapPolicyPartnerValue } from '../../../servicelog/servicelog.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'service-log-snap-policy-details',
  templateUrl: './service-log-snap-policy-details.component.html'
})
export class ServiceLogSnapPolicyDetailsComponent implements OnInit {
  @Input() snapPolicyId;
  snapPolicy:Observable<SnapPolicy>|null = null;
  partners:Observable<SnapPolicyPartner[]>;
  loading=false;
  constructor(
    private service:ServicelogService
  ) { 
    this.partners = service.snappolicypartner();
  }

  ngOnInit() {
    this.snapPolicy = this.service.getSnapPolicy(this.snapPolicyId);
  }
  getPartnerValue(snapPolicyPartnerValue:SnapPolicyPartnerValue[], partnerid){
    var val = snapPolicyPartnerValue.filter( v => v.snapPolicyPartnerId == partnerid);
    return val.length == 0 ? 0 : val[0].value;
  }
}
