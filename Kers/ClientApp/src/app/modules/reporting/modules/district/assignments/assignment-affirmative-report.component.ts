import { Component, OnInit, Input } from '@angular/core';
import { AffirmativeService } from '../../affirmative/affirmative.service';
import { Observable } from 'rxjs/Observable';
import { PlanningUnit } from '../../user/user.service';

@Component({
  selector: 'assignment-affirmative-report',
  templateUrl: './assignment-affirmative-report.component.html',
  styleUrls: ['./assignment-affirmative-report.component.css']
})
export class AssignmentAffirmativeReportComponent implements OnInit {

  @Input() districtId = 0;
  counties:Observable<PlanningUnit[]>;

  constructor(
    private service:AffirmativeService
  ) { }

  ngOnInit() {
    this.counties = this.service.countiesWithoutReport(this.districtId);
  }

}
