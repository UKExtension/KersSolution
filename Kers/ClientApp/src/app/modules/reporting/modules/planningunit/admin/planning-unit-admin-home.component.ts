import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { PlanningunitService } from '../planningunit.service';
import { Observable } from 'rxjs';
import { PlanningUnit } from '../../user/user.service';

@Component({
    selector: 'planning-unit-admin-home',
    templateUrl: './planning-unit-admin-home.component.html',
    styleUrls: ['./planning-unit-admin-home.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
