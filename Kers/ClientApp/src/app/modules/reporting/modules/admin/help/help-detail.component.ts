import {    Component, Input, OnInit, EventEmitter, Output   } from '@angular/core';
import { HelpService, Help } from './help.service';

@Component({
    selector: '[helpListDetail]',
    templateUrl: 'help-detail.component.html'
})
export class HelpDetailComponent implements OnInit {

    @Input ('helpListDetail') help: Help;

    @Output() onHelpUpdated = new EventEmitter();
    @Output() onHelpDeleted = new EventEmitter<Help>();

    editOppened = false;
    deleteOppened = false;
    rowOppened = true;
    errorMessage: string;


    constructor(private service: HelpService){
        
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
        this.service.deleteHelp(this.help.id).subscribe(
            res => {
                    this.onHelpDeleted.emit(this.help);
                },
            error =>  this.errorMessage = <any>error
        );
    }
    close(){
        this.rowOppened = true;
        this.editOppened = false;
        this.deleteOppened = false;
    }
    helpUpdated(event:Help){
        this.help = event;
        this.onHelpUpdated.emit();
        this.close();
    }

}