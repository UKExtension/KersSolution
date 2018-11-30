import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../../components/reporting/reporting.service';

@Component({
  selector: 'vehicle-district',
  template: `
    <p>
    <planningunit-list [link]="'/reporting/expense/vehicle/county/'"></planningunit-list>
    </p>
  `,
  styles: []
})
export class VehicleDistrictComponent implements OnInit {

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
