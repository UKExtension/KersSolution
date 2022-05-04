import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { PlanningUnit } from '../user/user.service';
import { ExtensionArea, ExtensionRegion, StateService } from './state.service';

@Component({
  selector: 'navigate-counties',
  templateUrl: './navigate-counties.component.html',
  styles: [
  ]
})
export class NavigateCountiesComponent implements OnInit {

  type="state"

  counties:Observable<PlanningUnit[]>;
  areas:Observable<ExtensionArea[]>;
  regions:Observable<ExtensionRegion[]>
  countiesOpen = true;
  areasOpen = false;
  regionsOpen = false;

  constructor(
    private service:StateService
  ) { }

  ngOnInit(): void {
    this.counties = this.service.counties();
    this.areas = this.service.areas();
    this.regions = this.service.regions();
  }
  openCounties(){
    this.countiesOpen = true;
    this.areasOpen = false;
    this.regionsOpen = false;
  }
  openAreas(){
    this.countiesOpen = false;
    this.areasOpen = true;
    this.regionsOpen = false;
  }
  openRegions(){
    this.countiesOpen = false;
    this.areasOpen = false;
    this.regionsOpen = true;
  }

}
