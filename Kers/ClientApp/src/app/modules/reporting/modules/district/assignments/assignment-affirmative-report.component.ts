import { Component, OnInit, Input } from '@angular/core';
import { AffirmativeService } from '../../affirmative/affirmative.service';
import { Observable } from 'rxjs';
import { PlanningUnit } from '../../plansofwork/plansofwork.service';

@Component({
  selector: 'assignment-affirmative-report',
  templateUrl: './assignment-affirmative-report.component.html',
  styleUrls: ['./assignment-affirmative-report.component.css']
})
export class AssignmentAffirmativeReportComponent implements OnInit {

  @Input() districtId = 0;
  @Input() areaId = 0;
  @Input() regionId = 0;
  @Input() type = 'district';
  @Input() fiscalYearId = "0";
  counties:Observable<PlanningUnit[]>;

  constructor(
    private service:AffirmativeService
  ) { }

  ngOnInit() {
    var id:number;
    if(this.type == "district"){
      id = this.districtId;
    }else if( this.type == "area"){
      id = this.areaId;
    }else{
      id = this.regionId;
    }
    this.counties = this.service.countiesWithoutReport(id, this.fiscalYearId, this.type);
  }

}
