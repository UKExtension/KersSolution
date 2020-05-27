import { Component, OnInit } from '@angular/core';
import { LadderApplication } from './ladder';
import { Observable, Subject } from 'rxjs';
import { startWith, flatMap, tap } from 'rxjs/operators';
import { LadderService, LadderApplicationSearchCriteria, LadderSeearchResultsWithCount } from './ladder.service';
import { ExtensionRegion, ExtensionArea, StateService } from '../state/state.service';
import { PlanningUnit } from '../user/user.service';

@Component({
  selector: 'ladder-filter',
  templateUrl: './ladder-filter.component.html',
  styles: []
})
export class LadderFilterComponent implements OnInit {

  criteria:LadderApplicationSearchCriteria;
  applications$: Observable<LadderSeearchResultsWithCount>;
  loading: boolean = true; // Turn spinner on and off
  refresh: Subject<string>; // For load/reload

  regions$:Observable<ExtensionRegion[]>;
  areas$:Observable<ExtensionArea[]>;
  counties$:Observable<PlanningUnit[]>;

  condition = false;


  constructor(
    private service:LadderService,
    private stateService:StateService
  ) {


    

   }

  ngOnInit() {
    this.regions$ = this.stateService.regions();
    this.counties$ = this.stateService.counties();


    this.criteria = {
      search: "",
      order: 'dsc',
      regionId: null,
      areaId: null,
      unitId: null,
      skip: 0,
      take: 20,
      fy: ""
    }

    


    this.refresh = new Subject();
    
    this.applications$ = this.refresh.asObservable()
          .pipe(
            startWith('onInit'), // Emit value to force load on page load; actual value does not matter
            flatMap(_ => this.service.getCustom(this.criteria)), // Get some items
            tap(_ => this.loading = false) // Turn off the spinner
          );



  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }

  onRegionChange(event){
    this.criteria.areaId = undefined;
    if(event.target.value == "null"){
      this.criteria.regionId = undefined;
    }else{
      this.criteria.regionId = <number>event.target.value;
      this.areas$ = this.stateService.areas(this.criteria.regionId);
    }
    this.onRefresh();
  }

  onAreaChange(event){
    if(event.target.value == "null"){
      this.criteria.areaId = undefined;
    }else{
      this.criteria.areaId = <number>event.target.value;
    } 
    this.onRefresh();
  }
  onCountyChange(event){
    if(event.target.value == "null"){
      this.criteria.unitId = undefined;
    }else{
      this.criteria["unitId"] = event.target.value;
    }
    this.onRefresh();
  }

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }

  getCount(count:number){
    return count;
  }


  loadMore(){
    this.criteria.take += 20;
    this.onRefresh();

  }

}
