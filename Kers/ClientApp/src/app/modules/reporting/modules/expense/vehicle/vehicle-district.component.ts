import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { ActivatedRoute, Params } from '@angular/router';

@Component({
  selector: 'vehicle-district',
  template: `
    <p *ngIf="districtId != null">
    <planningunit-list [districtId]="districtId" [link]="'/reporting/expense/vehicle/county/'"></planningunit-list>
    </p>
  `,
  styles: []
})
export class VehicleDistrictComponent implements OnInit {


  errorMessage:string;
  districtId:number = 0;
  constructor(
    private reportingService: ReportingService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.route.params.subscribe(
      (params: Params) => {
        if(params['id'] != undefined){
          this.districtId = params['id'];
        }
        
      },
      err => this.errorMessage = <any>err
    );
    
    this.defaultTitle()
  }
  defaultTitle(){
    this.reportingService.setTitle("Vehicles by County in the District");
}

}
