import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { ActivatedRoute, Params } from '@angular/router';

@Component({
  selector: 'vehicle-region',
  template: `
    <p *ngIf="regionId != null">
    <planningunit-list [regionId]="regionId" [link]="'/reporting/expense/vehicle/county/'"></planningunit-list>
    </p>
  `,
  styles: []
})
export class VehicleRegionComponent implements OnInit {


  errorMessage:string;
  regionId:number = 0;
  constructor(
    private reportingService: ReportingService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.route.params.subscribe(
      (params: Params) => {
        if(params['id'] != undefined){
          this.regionId = params['id'];
        }
        
      },
      err => this.errorMessage = <any>err
    );
    
    this.defaultTitle()
  }
  defaultTitle(){
    this.reportingService.setTitle("Vehicles by County in the Region");
}

}
