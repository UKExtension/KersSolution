import { Component, OnInit } from '@angular/core';
import { ReportingService } from '../../components/reporting/reporting.service';
import {PlansofworkService, Map} from './plansofwork.service';

@Component({
    selector: 'plansofwork-maps',
    templateUrl: 'plansofwork-maps.component.html' 
})
export class PlansofworkMapsComponent implements OnInit{

    maps:Map[];
    newMap = false;

    errorMessage: string;


    constructor(     
            private plansofworkService:PlansofworkService    
                ){
                    
                }
   
    ngOnInit(){
        this.plansofworkService.listMaps().subscribe(
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

    newMapSubmitted(){
        this.newMap = false;
    }

    onMapUpdate(){
        this.plansofworkService.listMaps().subscribe(
            maps => this.maps = maps,
            error =>  this.errorMessage = <any>error
        );
    }
    

}