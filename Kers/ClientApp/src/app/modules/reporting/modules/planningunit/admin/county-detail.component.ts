import { Component, OnInit, Input } from '@angular/core';
import { PlanningUnit } from '../../user/user.service';

@Component({
  selector: 'county-detail',
  templateUrl: './county-detail.component.html',
  styleUrls: ['./county-detail.component.css']
})
export class CountyDetailComponent implements OnInit {
  @Input() county:PlanningUnit
  rowDefault = true;
  rowEdit=false;

  constructor() { }

  ngOnInit() {
  }

  edit(){
    this.rowDefault = false;
    this.rowEdit = true;
  }
  default(){
    this.rowDefault = true;
    this.rowEdit = false;
  }
  updated(event:PlanningUnit){
    this.county = event;
    this.default();
  }

}
