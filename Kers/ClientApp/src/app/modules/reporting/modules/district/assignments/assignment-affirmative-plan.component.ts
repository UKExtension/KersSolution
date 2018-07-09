import { Component, OnInit, Input } from '@angular/core';
import { AffirmativeService } from '../../affirmative/affirmative.service';
import { Observable } from 'rxjs/Observable';
import { PlanningUnit } from '../../user/user.service';

@Component({
  selector: 'assignment-affirmative-plan',
  templateUrl: './assignment-affirmative-plan.component.html',
  styleUrls: ['./assignment-affirmative-plan.component.css']
})
export class AssignmentAffirmativePlanComponent implements OnInit {

  @Input() districtId = 0;
  counties:Observable<PlanningUnit[]>;

  constructor(
    private service:AffirmativeService
  ) { }

  ngOnInit() {
    this.counties = this.service.countiesWithoutPlan(this.districtId, "2019");
  }

}
