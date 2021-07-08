import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { PlanningunitService } from './planningunit.service';
import { PlanningUnit } from '../user/user.service';
import { Observable } from 'rxjs';
import { RegionService } from '../region/region.service';
import { AreaService } from '../area/area.service';


@Component({
    selector: 'planningunit-list',
    templateUrl: 'planningunit-list.component.html'
})
export class PlanningunitListComponent implements OnInit{ 
    

    @Input() justCounties:boolean = true;
    @Input() districtId:number | null = null;
    @Input() areaId:number | null = null;
    @Input() regionId:number | null = null;
    @Input() link:string;

    counties:Observable<PlanningUnit[]>;
    
    errorMessage: string;

    constructor( 
        private service:PlanningunitService,
        private areaService:AreaService,
        private regionService:RegionService
    )   
    {
        
    }

    ngOnInit(){
        if(this.areaId != null){
            this.counties = this.areaService.counties(this.areaId, true);
        }else if(this.regionId  != null){
            this.counties = this.regionService.counties(this.regionId);
        }else{
            this.counties = this.service.counties(this.districtId);
        }
    }


    

}