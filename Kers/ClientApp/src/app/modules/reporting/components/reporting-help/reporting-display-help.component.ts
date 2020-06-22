import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import {ReportingHelpService, Help} from './reporting-help.service';

@Component({
    selector: 'reporting-display-help',
    templateUrl: 'reporting-display-help.component.html'
})

export class ReportingDisplayHelpComponent implements OnInit{
    @Input() id: number;
    @Input() label:string = 'help';
    @Input() symbol:string = 'fa-question-circle';

    help:Help = null;
    errorMessage: string;

    constructor(
        private service: ReportingHelpService
    ){}

    ngOnInit(){
        this.service.get(this.id).subscribe(
            res => this.help  = <Help> res,
            error =>  this.errorMessage = <any>error
        );
    }
}