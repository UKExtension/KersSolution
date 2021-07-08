import { Component, OnInit, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { PlanningUnit } from '../../user/user.service';
import { IndicatorsService } from '../../indicators/indicators.service';

@Component({
  selector: 'assignment-program-indicators',
  templateUrl: './assignment-program-indicators.component.html',
  styleUrls: ['./assignment-program-indicators.component.css']
})
export class AssignmentProgramIndicatorsComponent implements OnInit {


  @Input() districtId = 0;
  @Input() areaId = 0;
  @Input() regionId = 0;
  @Input() type = 'district';
  @Input() fiscalYearId = "0";
  counties:Observable<PlanningUnit[]>;

  constructor(
    private service:IndicatorsService
  ) { }

  ngOnInit() {
    var id:number;
    if(this.type == "district"){
      id = this.districtId;
    }else if( this.type == "area"){
      id = this.areaId;
    }else{
      id = this.regionId;
    }
    this.counties = this.service.countiesWithoutIndicators(id, this.fiscalYearId, this.type);
  }

}
