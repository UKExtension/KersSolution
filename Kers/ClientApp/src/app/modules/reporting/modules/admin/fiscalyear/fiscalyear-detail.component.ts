import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { FiscalyearService, FiscalYear } from './fiscalyear.service';

@Component({
    selector: '[fiscalyearListDetail]',
    templateUrl: 'fiscalyear-detail.component.html'
})
export class FiscalyearDetailComponent implements OnInit {

    @Input ('fiscalyearListDetail') fiscalyear: FiscalYear;

    @Output() onFiscalyearUpdated = new EventEmitter();
    @Output() onFisclyearDeleted = new EventEmitter();

    editOppened = false;
    deleteOppened = false;
    rowOppened = true;
    errorMessage: string;


    constructor(private service: FiscalyearService){
        
    }

    ngOnInit(){
        
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
        this.service.deleteFiscalYear(this.fiscalyear.id).subscribe(
            res => {
                    this.onFisclyearDeleted.emit();
                    return res;
                },
            error =>  this.errorMessage = <any>error
        );
    }
    close(){
        this.rowOppened = true;
        this.editOppened = false;
        this.deleteOppened = false;
    }
    fiscalyearUpdated(){
        this.onFiscalyearUpdated.emit();
        this.close();
    }

}