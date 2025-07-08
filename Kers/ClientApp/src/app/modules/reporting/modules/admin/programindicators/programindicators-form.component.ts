import {Component, OnInit, Input, Output, EventEmitter} from '@angular/core';
import { ProgramsService, MajorProgram} from '../programs/programs.service';
import { FormBuilder, Validators }   from '@angular/forms';
import {IndicatorsService, Indicator} from '../../indicators/indicators.service';


@Component({
    selector: 'programindicators-form-admin',
    templateUrl: 'programindicators-form.component.html'
})
export class ProgramindicatorsFormComponent{

    @Input()majorProgram:MajorProgram;
    @Input()indicator:Indicator;

    errorMessage: string;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Indicator>();

    indicatorForm = null;

    public options: Object;

    constructor( 
        private fb: FormBuilder,
        private service:IndicatorsService
    )   
    {
        this.indicatorForm = fb.group(
            {
              question: ['', Validators.required],
              order: [1],
              isYouth: 0
            }
        );
        this.options = { 
            placeholderText: 'Program Indicator',
            toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL', '|', 'html'],
            toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'html'],
            toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
        }
    }

    ngOnInit(){
        if(this.indicator!=null){
            this.indicatorForm.patchValue(this.indicator);
        }else{
            this.service.indicatorsnextorder(this.majorProgram.id).subscribe(
                res => this.indicatorForm.patchValue({order:res})
            );
        }
        
    }

    onSubmit(){
        if(this.indicator){
            //edit indicator
            this.service.updateIndicator(this.indicator.id, this.indicatorForm.value).subscribe(
                res=>{
                            this.onFormSubmit.emit(<Indicator>res);
                        },
                        err => this.errorMessage = <any>err
            );
        }else{
            //new indicator
            this.service.
                addIndicator(this.majorProgram.id, this.indicatorForm.value).
                    subscribe(
                        res=>{
                            this.onFormSubmit.emit(<Indicator>res);
                        },
                        err => this.errorMessage = <any>err
                    );
            
        }
    }

    OnCancel(){
        this.onFormCancel.emit();
    }

    
}