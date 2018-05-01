import { Component, OnInit, Input } from '@angular/core';
import { PlansofworkService, PlanningUnit } from '../../plansofwork/plansofwork.service';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'assignment-plans-of-work',
  templateUrl: './assignment-plans-of-work.component.html',
  styleUrls: ['./assignment-plans-of-work.component.css']
})
export class AssignmentPlansOfWorkComponent implements OnInit {


  @Input() districtId = 0;
  counties:Observable<PlanningUnit[]>;

  constructor(
    private service:PlansofworkService
  ) { 
    
  }

  ngOnInit() {
    this.counties = this.service.countiesWithoutPlans(this.districtId);
  }

}
