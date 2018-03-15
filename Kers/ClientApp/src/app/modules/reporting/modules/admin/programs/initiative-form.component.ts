import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ProgramsService, StrategicInitiative, ProgramCategory } from './programs.service';
import {Location} from '@angular/common';
import { FormBuilder, Validators, FormControl } from '@angular/forms';
import {Router} from '@angular/router';
import { FiscalYear } from '../fiscalyear/fiscalyear.service';

@Component({
    selector: 'initiative-form',
    templateUrl: 'initiative-form.component.html' 
})
export class InitiativeFormComponent implements OnInit{

    initiativeForm = null;
    programCategories: ProgramCategory[] = null;
    @Input() initiative:StrategicInitiative = null;
    @Input() fiscalYear:FiscalYear;
    errorMessage: string;


    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private service: ProgramsService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location
    ){

        

        
    }
   
    ngOnInit(){
        this.initiativeForm = this.fb.group(
            {
              name: ['', Validators.required],
              programCategoryId: [0],
              pacCode: ['', this.validateNumber],
              order: ['', this.validateNumber]
            }
        );
        if(this.programCategories == null){
            this.service.categories().subscribe(
                res => {
                    this.programCategories = res
                    if(this.initiative){
                        this.initiativeForm.patchValue(this.initiative);
                        this.initiativeForm.patchValue({programCategory: this.initiative.programCategory.id});
                        
                    }
                },
                error => this.errorMessage = error
            );
       }
       

    }

    onSubmit(){    
        
        //var programCategoryId = this.initiativeForm.value.programCategory;
        //var cat = this.programCategories.find(c=>c.id = programCategoryId);
        var i = <StrategicInitiative> this.initiativeForm.value;
        //i.programCategory = cat[0];
        //i.programCategoryId = programCategoryId;
        //console.log(i);
        if(this.initiative){
            this.service.updateInitiative(this.initiative.id, i ).
            subscribe(
                res => {
                    
                    this.initiative = res;
                    this.onFormSubmit.emit();
                }
            );
        }else{
            this.service.addInitiative(i, this.fiscalYear).
            subscribe(
                res => {
                    this.onFormSubmit.emit();
                }
            );
        }
       
    }

    OnCancel(){
        this.onFormCancel.emit();
    }  

    validateNumber(c: FormControl) {
        return c.value > 0 && c.value < 100000 ? null : {valid: false}
    }; 
}