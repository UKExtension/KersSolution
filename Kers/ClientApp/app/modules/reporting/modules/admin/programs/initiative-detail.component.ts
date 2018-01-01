import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { ProgramsService, StrategicInitiative } from './programs.service';

@Component({
    selector: '[initiativeListDetail]',
    templateUrl: 'initiative-detail.component.html'
})
export class InitiativeDetailComponent implements OnInit {

    @Input ('initiativeListDetail') initiative: StrategicInitiative;

    @Output() onInitiativeUpdated = new EventEmitter();
    @Output() onInitiativeDeleted = new EventEmitter();

    editOppened = false;
    deleteOppened = false;
    programsOppened = false;
    rowOppened = true;
    errorMessage: string;


    constructor(private service: ProgramsService){
        
    }

    ngOnInit(){
        
    }
    programs(){
        this.rowOppened = false;
        this.programsOppened = true;
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
        this.service.deleteInitiative(this.initiative.id).subscribe(
            res => {
                    this.onInitiativeDeleted.emit();
                    return res;
                },
            error =>  this.errorMessage = <any>error
        );
        
    }
    close(){
        this.rowOppened = true;
        this.editOppened = false;
        this.deleteOppened = false;
        this.programsOppened = false;
    }
    initiativeUpdated(){
        this.onInitiativeUpdated.emit();
        this.close();
    }

}