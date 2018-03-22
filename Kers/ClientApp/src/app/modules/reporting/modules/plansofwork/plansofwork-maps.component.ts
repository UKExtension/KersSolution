import { Component, OnInit, Input } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';
import {PlansofworkService, Map} from './plansofwork.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: 'plansofwork-maps',
    templateUrl: 'plansofwork-maps.component.html' 
})
export class PlansofworkMapsComponent implements OnInit{
    @Input() fy:FiscalYear;

    maps:Map[];
    newMap = false;

    errorMessage: string;


    constructor(     
            private plansofworkService:PlansofworkService    
                ){
                    
                }
   
    ngOnInit(){
        this.plansofworkService.listMaps(this.fy.name).subscribe(
            maps => this.maps = maps,
            error =>  this.errorMessage = <any>error
        );
    }

    newMapOpen(){
        this.newMap = true;
    }

    newMapCancelled(){
        this.newMap = false;
    }

    newMapSubmitted(event){
        this.newMap = false;
    }

    onMapUpdate(){
        this.plansofworkService.listMaps(this.fy.name).subscribe(
            maps => this.maps = maps,
            error =>  this.errorMessage = <any>error
        );
    }
    

}