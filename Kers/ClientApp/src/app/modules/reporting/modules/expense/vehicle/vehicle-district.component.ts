import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { ActivatedRoute, Params } from '@angular/router';

@Component({
  selector: 'vehicle-district',
  template: `
    <p *ngIf="districtId">
    <planningunit-list [districtId]="districtId" [link]="'/reporting/expense/vehicle/county/'"></planningunit-list>
    </p>
  `,
  styles: []
})
export class VehicleDistrictComponent implements OnInit {


  errorMessage:string;
  districtId:number | null = null;
  constructor(
    private reportingService: ReportingService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.route.params.subscribe(
      (params: Params) => {
        this.districtId = params['id'];
      },
      err => this.errorMessage = <any>err
    );
    
    this.defaultTitle()
  }
  defaultTitle(){
    this.reportingService.setTitle("Vehicles by County in the District");
}

}
