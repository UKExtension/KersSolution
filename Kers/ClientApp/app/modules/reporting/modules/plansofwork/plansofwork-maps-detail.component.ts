import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { PlansofworkService, Map } from './plansofwork.service';

@Component({
    selector: '[plansofworkMapsDetail]',
    templateUrl: 'plansofwork-maps-detail.component.html'
})
export class PlansofworkMapsDetailComponent implements OnInit {

    @Input ('plansofworkMapsDetail') map: Map;

    @Output() onMapUpdated = new EventEmitter();
    @Output() onMapDeleted = new EventEmitter();

    loading = false;
    editOppened = false;
    deleteOppened = false;
    deleteAllowed = false;
    rowOppened = true;
    errorMessage: string;


    constructor(
        private plansofworkService: PlansofworkService){
        
    }

    ngOnInit(){
        var r = this.plansofworkService.
                    isMapDeleteAllowed(this.map.id).
                    subscribe(
                        has => {
                            this.deleteAllowed = !has;
                        },
                        error =>  this.errorMessage = <any>error);
    }
    edit(){
        this.rowOppened = false;
        this.editOppened = true;
    }
    delete(){
        this.rowOppened = false;
        this.deleteOppened = true;
    }
    confirmDelete(){
        this.loading = true;
        this.plansofworkService.deleteMap(this.map.id).subscribe(
            res => {
                    
                    this.onMapDeleted.emit();
                    this.loading = false;
                },
            error =>  this.errorMessage = <any>error
        );
    }
    close(){
        this.rowOppened = true;
        this.editOppened = false;
        this.deleteOppened = false;
    }
    mapUpdated(){
        this.onMapUpdated.emit();
        this.close();
    }
    mapDeleted(){
        this.onMapDeleted.emit();
    }

}