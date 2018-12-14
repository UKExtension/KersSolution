import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { PlanningunitService } from './planningunit.service';
import { PlanningUnit } from '../user/user.service';
import { Observable } from 'rxjs/Observable';


@Component({
    selector: 'planningunit-list',
    templateUrl: 'planningunit-list.component.html'
})
export class PlanningunitListComponent implements OnInit{ 
    

    @Input() justCounties:boolean = true;
    @Input() districtId:number | null = null;
    @Input() link:string;

    counties:Observable<PlanningUnit[]>;
    
    errorMessage: string;

    constructor( 
        private service:PlanningunitService
    )   
    {
        
    }

    ngOnInit(){
        this.counties = this.service.counties(this.districtId);
       //console.log(this.link);
    }


    

}