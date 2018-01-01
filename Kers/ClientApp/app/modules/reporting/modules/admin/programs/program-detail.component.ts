import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { ProgramsService, StrategicInitiative, MajorProgram } from './programs.service';

@Component({
    selector: '[programListDetail]',
    templateUrl: 'program-detail.component.html'
})
export class ProgramDetailComponent implements OnInit {

    @Input ('programListDetail') program: MajorProgram;

    @Output() onProgramUpdated = new EventEmitter();
    @Output() onProgramDeleted = new EventEmitter();

    editOppened = false;
    deleteOppened = false;
    rowOppened = true;
    errorMessage: string;


    constructor(private service: ProgramsService){
        
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
        
        this.service.deleteProgram(this.program.id).subscribe(
            res => {
                    this.onProgramDeleted.emit();
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
    programUpdated(){
        this.onProgramUpdated.emit();
        this.close();
    }

}