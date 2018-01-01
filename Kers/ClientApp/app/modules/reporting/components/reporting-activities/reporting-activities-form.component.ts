import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { ReportingActivitiesService, zActivity } from './reporting-activities.service';
import { ReportingService } from '../reporting/reporting.service';
import { FormBuilder, Validators, FormControl, FormGroup }   from '@angular/forms';
import { Router } from '@angular/router';
import { DatepickerModule } from 'angular2-material-datepicker';
import {Location} from '@angular/common';
import {ProfileService, Profile} from '../reporting-profile/profile.service';
import { ProgramsService, MajorProgram, StrategicInitiative } from '../../modules/admin/programs/programs.service';


@Component({
    selector: 'reporting-activities',
    templateUrl: 'reporting-activities-form.component.html',
    styles: [`


.datepicker input{
    border: 3px solie #111;
}
.fa{
    font-size: 16px;
}
    /* The switch - the box around the slider */
.switch {
  position: relative;
  display: inline-block;
  width: 60px;
  height: 34px;
}

/* Hide default HTML checkbox */
.switch input {display:none;}

/* The slider */
.slider {
  position: absolute;
  cursor: pointer;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: #ccc;
  -webkit-transition: .4s;
  transition: .4s;
}

.slider:before {
  position: absolute;
  content: "";
  height: 26px;
  width: 26px;
  left: 4px;
  bottom: 4px;
  background-color: white;
  -webkit-transition: .4s;
  transition: .4s;
}

input:checked + .slider {
  background-color: rgb(38, 185, 154);
  border-color: rgb(38, 185, 154); 
  box-shadow: rgb(38, 185, 154) 
}

input:focus + .slider {
  box-shadow: 0 0 1px rgb(38, 185, 154);
}

input:checked + .slider:before {
  -webkit-transform: translateX(26px);
  -ms-transform: translateX(26px);
  transform: translateX(26px);
}

/* Rounded sliders */
.slider.round {
  border-radius: 34px;
}

.slider.round:before {
  border-radius: 50%;
}
    
.snap-audience input{
    width: 50px;
} 
    
    `] 
})
export class ReportingActivitiesFormComponent implements OnInit, OnDestroy{

    activity: zActivity;
    errorMessage: string;

    public options: Object;

    date: Date;

    form = null;
    raceEthnicityFormGroup = null;
    snapEdFormGroup = null;
    pacs = null;
    days = null;
    snapEd = false;
    genderRatio = 0.6;
    participantsNum = 0;
    femaleGhanged = false;

    programs: MajorProgram[];
    initiatives:StrategicInitiative[];
    

    @Input () id: number;


    constructor( 
        private activitiesService: ReportingActivitiesService,
        private reportingService: ReportingService,
        private profileService: ProfileService,
        private fb: FormBuilder,
        private router: Router,
        private location: Location,
        private service:ProgramsService
    ){

        this.participantsNum = 0;

        this.options = { 
            placeholderText: 'Description',
            toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL','insertImage', '|', 'html'],
            toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'html'],
            toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
            imageUploadURL: location.prepareExternalUrl('/FroalaApi/UploadImage'),
            fileUploadURL: location.prepareExternalUrl('/FroalaApi/UploadFile'),
            imageManagerLoadURL: location.prepareExternalUrl('/FroalaApi/LoadImages'),
            imageManagerDeleteURL: location.prepareExternalUrl('/FroalaApi/DeleteImage'),
            imageManagerDeleteMethod: "POST"
        }

    }
   
    ngOnInit(){
        this.reportingService.setTitle("Service Log");
        this.reportingService.setSubtitle("for 2016-2017 (FY2017)");


        this.raceEthnicityFormGroup = this.fb.group({
                  raceWhite: [''],
                  hRaceWhite: [''],
                  raceBlack: [''],
                  hRanceBlack: [''],
                  raceAsian: [''],
                  hRaceAsian: [''],
                  raceIndian: [''],
                  hRaceIndian: [''],
                  raceHawaiian: [''],
                  hRaceHawaiian: [''],
                  raceCannotBeDetermined: [''],
                  hRaceCannotBeDetermined: [''],
                  raceOther: [''],
                  hRaceOther: ['']
              });
        this.snapEdFormGroup = this.fb.group({

        });

        this.form = this.fb.group(
            {
              rDT: [''],
              activityTitle: ['', Validators.required],
              activityDescription: ['', Validators.required],
              activityDays: ['', Validators.required],
              notPresent: [''],
              night: [''],
              weekend: [''],
              notExtensionSponsored: [''],
              multiState: [''],
              MS4: [''],
              genderFemale: [ '', this.isGenderValid ],
              raceEthnicity: this.raceEthnicityFormGroup,
              majorPrograms: ""
            });
        this.setToday();

/*
        this.activitiesService.days().subscribe(
            d => { this.days = d },
            error => this.errorMessage = <any> error
        );
*/
        var prgrms = [];
        this.service.listInitiatives().subscribe(
            i => {
                this.initiatives = i;
                
                i.forEach(
                    function(initiative) {
                        
                        initiative.majorPrograms.forEach(
                            function(program){
                                prgrms.push(program);
                            }
                        )
                    }
                    
                )
                this.programs = prgrms;
            },
            error =>  this.errorMessage = <any>error
        );


    }
    isBad( control:string){
        if(this.form.get(control).hasError('required') && this.form.get(control).touched) return true;
        return false;
    }

    ngOnDestroy(){
        this.reportingService.setSubtitle("");
    }


    onSnapEdChecked(){
        this.snapEd = !this.snapEd;
        //console.log(this.snapEd);
    }

    onSubmit(){
        
        this.reportingService.setAlert("Activity Submitted");
        this.router.navigate(['/reporting']);
    }

    setToday() {
        this.date = new Date();
    }
        

    totals(formGroup: FormGroup, controls?: string[] ):number{
        var total = 0;
        if(controls == null ){
            for( let control in formGroup.controls ){
                total += Math.max( formGroup.get(control).value, 0 );
            }
        }else{
            controls.forEach(element => {
                total += Math.max( formGroup.get(element).value, 0 );
            });
        }
        return total;
    }

    totalRace(){
        this.participantsNum = this.totals(this.raceEthnicityFormGroup);
        this.setFemaleValue();
        return this.participantsNum ;
    }

    numParticipants(): number{
            return this.participantsNum;
    }

    femalesPrestine(){
        return Math.floor( this.numParticipants() * this.genderRatio );
    }

    setFemaleValue(){
        if( !this.form.get('genderFemale').dirty ){
            this.form.get('genderFemale').setValue(this.femalesPrestine());
        }else if(this.form.get('genderFemale').value < 0 ){
            this.form.get('genderFemale').setValue(0);
        }
    }

    males(){
        return Math.max(this.participantsNum  - this.form.get('genderFemale').value, 0);
    }

    isGenderValid(control: FormControl){
        /*
        TODO Figure out how to validate that gender number is matching the total num of participants
        */
        return  null;
    }
}