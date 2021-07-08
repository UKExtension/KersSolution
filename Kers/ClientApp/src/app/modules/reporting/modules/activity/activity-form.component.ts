import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { IMyDpOptions, IMyDateModel } from "mydatepicker";
import { FormBuilder, Validators, FormControl, AbstractControl } from "@angular/forms";
import {    ActivityService, Activity, 
            ActivityOption, ActivityOptionNumber, 
            ActivityOptionNumberValue, ActivityOptionSelection,
            Race, Ethnicity, RaceEthnicityValue
        } from './activity.service';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { Observable } from "rxjs";



@Component({
    selector: 'activity-form',
    templateUrl: 'activity-form.component.html',
    styleUrls: ['activity-form.component.scss'] 
})
export class ActivityFormComponent implements OnInit{ 

    @Input() activity:Activity = null;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Activity>();

    loading = true;
    activityForm = null;

    optionArray:ActivityOption[];
    races:Race[];
    ethnicities:Ethnicity[];
    optionNumbers:ActivityOptionNumber[];

    snapEligable = false;
    hasIndirect = false;
    hasDirect = false;

    raceEthnicityIndex = 0;

    genderRatio = 0.6;
    participantsNum = 0;
    femaleGhanged = false;


    options:object;
    errorMessage:string;
    initiatives:StrategicInitiative[];

    snapFiscalYear18 = false;

    private myDatePickerOptions: IMyDpOptions = {
        // other options...
            dateFormat: 'mm/dd/yyyy',
            showTodayBtn: false,
            satHighlight: true
        };

    constructor( 
        private fb: FormBuilder,
        private service: ActivityService,
        private programsService:ProgramsService
    )   
    {
          
        this.options = { 
            placeholderText: 'Your Description Here!',
            toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL'],
            toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
            quickInsertButtons: ['ul', 'ol', 'hr'],    
        }


    }

    ngOnInit(){
        this.programsService.listInitiatives().subscribe(
            i => this.initiatives = i,
            error =>  this.errorMessage = <any>error
        );
        this.populateOptions();
    }


    //Disable Snap Ed Checkbox for the 2018 fiscal year on date change
    onDateChanged(event: IMyDateModel) {
        if(event.date.year >= 2017 && event.date.month > 9){
            this.snapFiscalYear18 = true;
            this.activityForm.patchValue({isSnap:false});
            this.snapEligable = false;
        }else{
            this.snapFiscalYear18 = false;
        }
    }

    populateOptions(){
        this.service.options().subscribe(
            res=>{
                this.optionArray = <ActivityOption[]> res;
                this.populateOptionNumbers();
            },
            err => this.errorMessage = <any> err
        );
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
        if(this.raceEthnicityIndex == this.activityForm.get("raceEthnicityValues").length){
            this.raceEthnicityIndex = 0
        }
        var val = ''+this.raceEthnicityIndex++
        return val;
    }

    totalRace(){
        var sum = 0;
        for( let contr of this.activityForm.controls.raceEthnicityValues.controls){
            sum += contr.value.amount;
        }   
        this.participantsNum = sum;
        this.setFemaleValue();
        return sum;
    }
    totalRaceById(raceId:number){
        var sum = 0;
        for( let contr of this.activityForm.controls.raceEthnicityValues.controls){
            if(contr.value.raceId == raceId){
                sum += contr.value.amount;
            }
        }   
        return sum;
    }
    audienceTotal(fields:string[]){
        var sum = 0;
        for(let field of fields){
            let cntr = this.activityForm.controls.snapClassic.controls.direct.get(field);
            if( !isNaN(parseInt(cntr.value))){
                sum += parseInt(cntr.value);
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
        if( !this.activityForm.get('female').dirty ){
            this.activityForm.get('female').setValue(this.femalesPrestine());
        }else if(this.activityForm.get('female').value < 0 ){
            this.activityForm.get('female').setValue(0);
        }
    }

    males(){
        var m = Math.max(this.participantsNum  - this.activityForm.get('female').value, 0);
        this.activityForm.get('male').setValue(m);
        return m;
    }

    generateForm(){
        let opArray = [];
        for( let option of this.optionArray){
            opArray.push(this.fb.group({
                selected: false,
                activityOptionId: option.id
            }));
        }
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
        this.activityForm = this.fb.group(
            {
            
                activityDate: [{
                                date: {
                                    year: date.getFullYear(),
                                    month: date.getMonth() + 1,
                                    day: date.getDate()}
                                }, Validators.required],
                
                title: ["", Validators.required],
                description: ["", Validators.required],
                majorProgramId: ["", Validators.required],
                hours: ["", Validators.required],
                activityOptionSelections: this.fb.array(opArray),
                raceEthnicityValues:this.fb.array(raceEthnArray),
                female:[0, this.isPositiveInt],
                male:0,
                activityOptionNumbers:this.fb.array(opNumArray),
                isSnap: false,
                snapClassic: this.fb.group(
                    {

                        direct: this.fb.group({
                                snapDirectDeliverySiteID: [""],
                                snapDirectSpecificSiteName: "",
                                snapDirectSessionTypeID: [""],
                                snapDirectAudience_00_04_FarmersMarket:[0, this.isPositiveInt],
                                snapDirectAudience_05_17_FarmersMarket: [0, this.isPositiveInt],
                                snapDirectAudience_18_59_FarmersMarket:[0, this.isPositiveInt],
                                snapDirectAudience_60_pl_FarmersMarket:[0, this.isPositiveInt],
                                snapDirectAudience_00_04_PreSchool:[0, this.isPositiveInt],
                                snapDirectAudience_05_17_PreSchool:[0, this.isPositiveInt],
                                snapDirectAudience_18_59_PreSchool:[0, this.isPositiveInt],
                                snapDirectAudience_60_pl_PreSchool:[0, this.isPositiveInt],
                                snapDirectAudience_00_04_Family:[0, this.isPositiveInt],
                                snapDirectAudience_05_17_Family:[0, this.isPositiveInt],
                                snapDirectAudience_18_59_Family:[0, this.isPositiveInt],
                                snapDirectAudience_60_pl_Family:[0, this.isPositiveInt],
                                snapDirectAudience_00_04_SchoolAge:[0, this.isPositiveInt],
                                snapDirectAudience_05_17_SchoolAge:[0, this.isPositiveInt],
                                snapDirectAudience_18_59_SchoolAge:[0, this.isPositiveInt],
                                snapDirectAudience_60_pl_SchoolAge:[0, this.isPositiveInt],
                                snapDirectAudience_00_04_LimitedEnglish:[0, this.isPositiveInt],
                                snapDirectAudience_05_17_LimitedEnglish: [0, this.isPositiveInt],
                                snapDirectAudience_18_59_LimitedEnglish: [0, this.isPositiveInt],
                                snapDirectAudience_60_pl_LimitedEnglish:[0, this.isPositiveInt],
                                snapDirectAudience_00_04_Seniors: [0, this.isPositiveInt],
                                snapDirectAudience_05_17_Seniors: [0, this.isPositiveInt],
                                snapDirectAudience_18_59_Seniors: [0, this.isPositiveInt],
                                snapDirectAudience_60_pl_Seniors: [0, this.isPositiveInt]
                            }),
                        indirect: this.fb.group({
                                snapIndirectEstNumbReachedPsaRadio: [0, this.isPositiveInt],
                                snapIndirectEstNumbReachedPsaTv: [0, this.isPositiveInt],
                                snapIndirectEstNumbReachedArticles: [0, this.isPositiveInt],
                                snapIndirectEstNumbReachedGroceryStore: [0, this.isPositiveInt],
                                snapIndirectEstNumbReachedFairsParticipated: [0, this.isPositiveInt],
                                snapIndirectEstNumbReachedFairsSponsored:[0, this.isPositiveInt],
                                snapIndirectEstNumbReachedNewsletter: [0, this.isPositiveInt],
                                snapIndirectEstNumbReachedSocialMedia: [0, this.isPositiveInt],
                                snapIndirectEstNumbReachedOther: [0, this.isPositiveInt],
                                snapIndirectMethodFactSheets: false,
                                snapIndirectMethodPosters: false,
                                snapIndirectMethodCalendars: false,
                                snapIndirectMethodPromoMaterial: false,
                                snapIndirectMethodWebsite:false,
                                snapIndirectMethodEmail:false,
                                snapIndirectMethodVideo:false,
                                snapIndirectMethodOther:false,
                        }),
                        snapCopies: [0, this.isPositiveInt]

                    }
                )
            }, { validator: snapValidator }
        );
        this.myDatePickerOptions.disableSince = {year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() + 1};
        this.myDatePickerOptions.disableUntil = {year: 2017, month: 6, day: 30};
        this.myDatePickerOptions.editableDateField = false;
        this.myDatePickerOptions.showClearDateBtn = false;
        


        if(this.activity == null){
            var dtNow = new Date();
            
            if( dtNow.getFullYear() >= 2017 && dtNow.getMonth() >= 9){
                this.snapFiscalYear18 = true;
            }else{
                this.snapFiscalYear18 = false;
            }
        }else{
            let date = new Date(this.activity.activityDate);
            if( date.getFullYear() >= 2017 && date.getMonth() >= 9){
                this.snapFiscalYear18 = true;
                this.activityForm.patchValue({isSnap:false});
            }else{
                this.snapFiscalYear18 = false;
            }
            this.patch();
        }


        

        this.loading = false;
    }

    patch(){

        let date = new Date(this.activity.activityDate);
        this.activity.activityDate = date;
        this.activityForm.get('female').markAsDirty();
        this.activityForm.patchValue(this.activity);
        let opArray = [];
        for( let option of this.optionArray){
            opArray.push({
                selected: this.activity.activityOptionSelections.filter(s => s.activityOptionId == option.id).length != 0,
                activityOptionId: option.id
            });
        }
        
        this.activityForm.patchValue({activityOptionSelections:opArray});
        this.activityForm.patchValue({activityDate: {
                date: {
                    year: date.getFullYear(),
                    month: date.getMonth() + 1,
                    day: date.getDate()}
                }});
        if(this.activity.isSnap){
            this.snapEligable = true;
        }
        
        if(this.activity.activityOptionNumbers.filter(n => n.activityOptionNumberId == 3)[0].value > 0){
            this.hasIndirect = true;
        }
        

        if(this.activity.isSnap){
            if(this.activity.classicSnapId != null && this.activity.classicSnapId != 0){
                
            }
            if(this.activity.classicIndirectSnapId != null && this.activity.classicIndirectSnapId != 0){
                
            }
        }


    }

    

    onSubmit(){
        this.loading = true;
        var dateValue = this.activityForm.value.activityDate.date;
        var d = new Date(Date.UTC(dateValue.year, dateValue.month - 1, dateValue.day, 8, 5, 12));
        /*
        d.setMonth(dateValue.month - 1);
        d.setDate(dateValue.day);
        d.setFullYear(dateValue.year);
        d.setHours(8);
        */
        var val = this.activityForm.value;
        val.activityDate = d;

        var actOpt = val.activityOptionSelections;
        val.activityOptionSelections = actOpt.filter(x => x.selected).map(x => { return { activityOptionId: x.activityOptionId }; });


        let m = (dateValue.month);
        let y = (dateValue.day)
        val.snapClassic.indirect.snapDate = val.snapClassic.direct.snapDate = '' + dateValue.year + (m<=9 ? '0' + m : m) + (y<=9 ? '0' + y : y);
        val.snapClassic.indirect.snapHours = val.snapClassic.direct.snapHours = val.hours + '.0';
        

       
    }

    onCancel(){
        this.onFormCancel.emit();
    }

    

    /************************
      
      SNAP ED methods
    
     ***********************/
    onSnapEdChecked(){
        this.snapEligable = !this.snapEligable;
    }

     onOptionNumberChange(event){
         if(event.target.id == "3"){
            if(!isNaN(event.target.value) && (function(x) { return (x | 0) === x; })(parseFloat(event.target.value)) && +event.target.value > 0){
                this.hasIndirect = true;
            }else{
                this.hasIndirect = false;
            }
        }
     }


     /************************
      
      Validators
    
     ***********************/

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



export const snapValidator = (control: AbstractControl): {[key: string]: boolean} => {
    const isSnap = control.get('isSnap');
    //const confirm = control.get('confirm');
    if (isSnap.value === false) return null;

    let male = control.get('male');
    let female = control.get('female');

    if(parseInt(male.value) + parseInt(female.value) <= 0 ){
        return null;
    }



    let site = control.get('snapClassic').get('direct').get('snapDirectDeliverySiteID');
    let session = control.get('snapClassic').get('direct').get('snapDirectSessionTypeID');
    if(!site.value && !session.value){
        return { nosite: true, nosession: true };
    }else if(!site.value){
        return { nosite: true };
    }else if(!session.value){
        return { nosession: true };
    }

    if(site.value == "1021"){
        let specificSite = control.get('snapClassic').get('direct').get('snapDirectSpecificSiteName');
        if(!specificSite.value){
            return { nospecificSite: true };
        }
    }

    return null;
};