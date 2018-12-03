import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../../components/reporting/reporting.service';

@Component({
  selector: 'vehicle',
  template: `
    <p>
    <planningunit-list [link]="'/reporting/expense/vehicle/county/'"></planningunit-list>
    </p>
  `,
  styles: []
})
export class VehicleComponent implements OnInit {

  constructor(
    private reportingService: ReportingService,
  ) { }

  ngOnInit() {
    this.defaultTitle()
  }
  defaultTitle(){
    this.reportingService.setTitle("Vehicles by County");
  }

}
