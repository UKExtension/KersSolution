import { Component, OnInit, Input } from '@angular/core';
import { PlansofworkService, PlanningUnit } from '../../plansofwork/plansofwork.service';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'assignment-plans-of-work',
  templateUrl: './assignment-plans-of-work.component.html',
  styleUrls: ['./assignment-plans-of-work.component.css']
})
export class AssignmentPlansOfWorkComponent implements OnInit {


  @Input() districtId:number = 0;
  @Input() areaId = 0;
  @Input() regionId = 0;
  @Input() type = 'district';
  @Input() fiscalYearId = "0";
  counties:Observable<PlanningUnit[]>;
  constructor(
    private service:PlansofworkService
  ) { 
    
  }

  ngOnInit() {
    var id:number;
    if(this.type == "district"){
      id = this.districtId;
    }else if( this.type == "area"){
      id = this.areaId;
    }else{
      id = this.regionId;
    }
    this.counties = this.service.countiesWithoutPlans(id, this.fiscalYearId, this.type);
  }

}
