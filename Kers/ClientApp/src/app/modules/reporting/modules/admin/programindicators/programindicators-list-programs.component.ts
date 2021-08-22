import {Component, OnInit, Input} from '@angular/core';
import { MajorProgram} from '../programs/programs.service';



@Component({
    selector: 'programindicators-programs-admin',
    templateUrl: `programindicators-list-programs.component.html`,
    styles: [`
        .row{
            padding-top: 20px;
            padding-bottom: 20px;
            border-bottom: 1px solid #D9DEE4;
            margin: 0;
        }
    `]
})
export class ProgramindicatorsListProgramsComponent implements OnInit{
    
    @Input()programs: MajorProgram[];

    constructor(
        
    ){}
    
    
    
    ngOnInit(){
    }

}