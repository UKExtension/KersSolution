import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../../components/reporting/reporting.service';
import { ActivatedRoute, Params } from '@angular/router';

@Component({
  selector: 'vehicle-area',
  template: `
    <p *ngIf="areaId != null">
    <planningunit-list [areaId]="areaId" [link]="'/reporting/expense/vehicle/county/'"></planningunit-list>
    </p>
  `,
  styles: []
})
export class VehicleAreaComponent implements OnInit {


  errorMessage:string;
  areaId:number = 0;
  constructor(
    private reportingService: ReportingService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.route.params.subscribe(
      (params: Params) => {
        if(params['id'] != undefined){
          this.areaId = params['id'];
        }
        
      },
      err => this.errorMessage = <any>err
    );
    
    this.defaultTitle()
  }
  defaultTitle(){
    this.reportingService.setTitle("Vehicles by County in the Area");
}

}
