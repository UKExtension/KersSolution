import { Component, Input, Output, EventEmitter } from '@angular/core';
import { TrainingService } from "./training.service";
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import { Training, TrainingSearchCriteria } from './training';

@Component({
    selector: '[training-detail]',
    templateUrl: 'training-detail.component.html'
})
export class TrainingDetailComponent { 
    rowDefault =true;
    rowEdit = false;
    rowDelete = false;
    currentFiscalYear:FiscalYear | null = null;
    displayEdit = false;

    
    @Input('training-detail') training:Training;
    @Input() admin:boolean = false;
    @Input() criteria:TrainingSearchCriteria;

    @Output() onDeleted = new EventEmitter<Training>();
    @Output() onEdited = new EventEmitter<Training>();
    
    errorMessage: string;

    constructor( 
        private service:TrainingService
    )   
    {}

    ngOnInit(){
    }
    edit(){
        this.rowDefault = false;
        this.rowEdit = true;
        this.rowDelete = false;
    }
    delete(){
        this.rowDefault = false;
        this.rowEdit = false;
        this.rowDelete = true;
    }
    default(){
        this.rowDefault = true;
        this.rowEdit = false;
        this.rowDelete = false;
    }

    trainingSubmitted(training:Training){
        this.training = training;
        this.onEdited.emit(training);
        this.default();
    }

    confirmDelete(){
        
        this.service.delete(this.training.id).subscribe(
            res=>{
                this.onDeleted.emit(this.training);
            },
            err => this.errorMessage = <any> err
        );
        
    }
    
}