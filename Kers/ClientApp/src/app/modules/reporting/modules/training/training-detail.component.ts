import { Component, Input, Output, EventEmitter } from '@angular/core';
import { TrainingService } from "./training.service";
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import { Training } from './training';
import { UserService, User } from '../user/user.service';
import { Observable } from 'rxjs';

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

    @Output() onDeleted = new EventEmitter<Training>();
    @Output() onEdited = new EventEmitter<Training>();
    
    errorMessage: string;

    constructor( 
        private service:TrainingService,
        private userService: UserService
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
    getUser(id:number):Observable<User>{
        return this.userService.byId(id);
    }
    
}