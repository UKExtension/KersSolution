import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { IMyDpOptions } from "mydatepicker";
import { FormBuilder, Validators, FormControl } from "@angular/forms";
import {    ContactService, Contact, 
            ContactOptionNumberValue,
            ContactRaceEthnicityValue
        } from './contact.service';
import {ActivityOptionNumber, Race, Ethnicity} from '../activity/activity.service';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { Observable } from "rxjs/Observable";



@Component({
    selector: 'contact-form',
    templateUrl: 'contact-form.component.html'
})
export class ContactFormComponent implements OnInit{ 

    @Input() contact:Contact = null;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Contact>();

    loading = true;
    contactForm = null;

    races:Race[];
    ethnicities:Ethnicity[];
    optionNumbers:ActivityOptionNumber[];

    raceEthnicityIndex = 0;

    genderRatio = 0.6;
    participantsNum = 0;
    femaleGhanged = false;


    options:object;
    errorMessage:string;
    initiatives:StrategicInitiative[];


    constructor( 
        private fb: FormBuilder,
        private service: ContactService,
        private programsService:ProgramsService,
    )   
    {}

    ngOnInit(){
        this.programsService.listInitiatives().subscribe(
            i => this.initiatives = i,
            error =>  this.errorMessage = <any>error
        );
        this.populateOptionNumbers();
    }




    populateOptionNumbers(){
        this.service.optionnumbers().subscribe(
            res => {
                this.optionNumbers = <ActivityOptionNumber[]>res;
                this.populateRaceEthnicity();
            },
            err => this.errorMessage = <any>err
        );
    }

    populateRaceEthnicity(){
        this.service.races().subscribe(
            res=>{
                this.races = <Race[]>res;
                this.service.ethnicities().subscribe(
                    res => {
                        this.ethnicities = <Ethnicity[]>res;
                        this.generateForm();
                    },
                    err => this.errorMessage = <any>res
                );
            },
            err => this.errorMessage = <any>err
        );
    }

    rIndex(){
        if(this.raceEthnicityIndex == this.contactForm.get("contactRaceEthnicityValues").length){
            this.raceEthnicityIndex = 0
        }
        var val = ''+this.raceEthnicityIndex++
        return val;
    }

    totalRace(){
        var sum = 0;
        for( let contr of this.contactForm.controls.contactRaceEthnicityValues.controls){
            sum += contr.value.amount;
        }   
        this.participantsNum = sum;
        this.setFemaleValue();
        return sum;
    }
    totalRaceById(raceId:number){
        var sum = 0;
        for( let contr of this.contactForm.controls.contactRaceEthnicityValues.controls){
            if(contr.value.raceId == raceId){
                sum += contr.value.amount;
            }
        }   
        return sum;
    }

    numParticipants(): number{
            return this.participantsNum;
    }

    femalesPrestine(){
        return Math.floor( this.numParticipants() * this.genderRatio );
    }

    setFemaleValue(){
        if( !this.contactForm.get('female').dirty ){
            this.contactForm.get('female').setValue(this.femalesPrestine());
        }else if(this.contactForm.get('female').value < 0 ){
            this.contactForm.get('female').setValue(0);
        }
    }

    males(){
        var m = Math.max(this.participantsNum  - this.contactForm.get('female').value, 0);
        this.contactForm.get('male').setValue(m);
        return m;
    }

    generateForm(){
        
        let opNumArray = [];
        for( let option of this.optionNumbers){
            opNumArray.push(this.fb.group({
                value: [0, this.isPositiveInt],
                activityOptionNumberId: option.id
            }));
        }
        let raceEthnArray = [];
        for(let rc of this.races){
            for(let et of this.ethnicities){
                raceEthnArray.push(this.fb.group({
                    amount: [0, this.isPositiveInt],
                    raceId: rc.id,
                    ethnicityId: et.id
                }));                
            }
        }


        let date = new Date();
        this.contactForm = this.fb.group(
            {
                contactDate:["", Validators.required],
                days: ["", this.isIntOrFloat],
                multistate: ["", this.isIntOrFloat],
                majorProgramId: ["", Validators.required],
                contactRaceEthnicityValues:this.fb.array(raceEthnArray),
                female:[0, this.isPositiveInt],
                male:0,
                contactOptionNumbers:this.fb.array(opNumArray)
            }
        );
        

        if(this.contact != null){
            this.patch();
        }

        this.loading = false;
    }

    patch(){

        this.contactForm.get('female').markAsDirty();
        this.contactForm.patchValue(this.contact);
        let date = new Date(this.contact.contactDate);
        this.contactForm.patchValue({contactDate:date.getMonth() + 1})
    }


    onSubmit(){


        var dateValue = this.contactForm.value.contactDate;
        var d = new Date();
        d.setMonth(dateValue - 1);
        d.setFullYear(+dateValue > 6 ? 2017 : 2018);
        var val = this.contactForm.value;
        val.contactDate = d;
        if(this.contact != null){
            this.service.update(this.contact.id, val).subscribe(
                res => {
                    this.loading = false;
                    this.onFormSubmit.emit(<Contact>res);
                },
                err => this.errorMessage = <any>err
            );
        }else{

            
            this.service.add(val).subscribe(
                res => {
                    var cntct = <Contact>res;
                    this.onFormSubmit.emit(cntct);
                    return cntct;
                },
                err => this.errorMessage = <any> err
            );
        }
    }

    onCancel(){
        this.onFormCancel.emit();
    }

    isIntOrFloat(control:FormControl){
        if(control.value == +control.value && +control.value >= 0){
            return null;
        }
        return {"notDigit":true};
    }

    isPositiveInt(control:FormControl){
        
        if(!isNaN(control.value) && (function(x) { return (x | 0) === x; })(parseFloat(control.value)) && +control.value >= 0){
            return null;
        }
        return {"notInt":true};
    }



}