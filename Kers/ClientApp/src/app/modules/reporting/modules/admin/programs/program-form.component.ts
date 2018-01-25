import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { ProgramsService, StrategicInitiative, MajorProgram } from './programs.service';
import {Location} from '@angular/common';
import { FormBuilder, Validators, FormControl } from '@angular/forms';
import {Router} from '@angular/router';

@Component({
    selector: 'program-form',
    templateUrl: 'program-form.component.html' 
})
export class ProgramFormComponent implements OnInit{

    programForm = null;
    @Input() program:MajorProgram = null;
    @Input() initiative:StrategicInitiative = null;
    errorMessage: string;


    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private service: ProgramsService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location
    ){

        

        this.programForm = fb.group(
            {
              name: ['', Validators.required],
              pacCode: ['', this.validateNumber],
              order: ['', this.validateNumber]
            }
        );
    }
   
    ngOnInit(){
       if(this.program){
           this.programForm.patchValue(this.program);
       }

    }

    onSubmit(){    
                
        if(this.program){
            this.service.updateProgram(this.program.id, this.programForm.value).
            subscribe(
                res => {
                    this.program = <MajorProgram> res;
                    this.onFormSubmit.emit();
                }
            );
        }else{
            this.service.addProgram(this.initiative, this.programForm.value).
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