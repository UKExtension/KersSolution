import { Component, OnInit } from '@angular/core';
import { PlanningunitService } from '../planningunit.service';
import { Observable } from 'rxjs/Observable';
import { PlanningUnit } from '../../user/user.service';

@Component({
  selector: 'planning-unit-admin-home',
  templateUrl: './planning-unit-admin-home.component.html',
  styleUrls: ['./planning-unit-admin-home.component.css']
})
export class PlanningUnitAdminHomeComponent implements OnInit {

  counties:Observable<PlanningUnit[]>;

  constructor(
    private service:PlanningunitService
  ) { }

  ngOnInit() {
    this.counties = this.service.counties();
  }

}
