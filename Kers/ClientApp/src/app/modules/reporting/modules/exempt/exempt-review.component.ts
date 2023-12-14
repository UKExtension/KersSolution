import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';
import { PlanningUnit } from '../user/user.service';
import { PlanningunitService } from '../planningunit/planningunit.service';
import { Observable} from 'rxjs';
import { TaxExempt } from './exmpt';
import { ExemptService } from './exempt.service';

@Component({
  selector: 'app-exempt-review',
  templateUrl: './exempt-review.component.html',
  styles: [
  ]
})
export class ExemptReviewComponent implements OnInit {
  planningUnits$: Observable<PlanningUnit[]>;
  exempts$:Observable<TaxExempt[]>;

  constructor(
    private planningUnitService: PlanningunitService,
    private reportingService: ReportingService,
    private service:ExemptService
  ) { }

  ngOnInit(): void {
    this.planningUnits$ = this.planningUnitService.counties();
    this.defaultTitle();
  }

  defaultTitle(){
    this.reportingService.setTitle("Tax Exempt/Volunteer Entities Management");
    //this.reportingService.setSubtitle("For specific In-Service related questions or assistance, please email: agpsd@lsv.uky.edu");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }
  changeCounty(event){
    this.exempts$ = this.service.exemptsList(event.target.value);
  }

}
