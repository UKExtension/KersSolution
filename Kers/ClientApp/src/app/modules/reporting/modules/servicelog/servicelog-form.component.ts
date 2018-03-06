import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { IMyDpOptions, IMyDateModel } from "mydatepicker";
import { FormBuilder, Validators, FormControl, AbstractControl } from "@angular/forms";
import {    ActivityService, Activity, 
            ActivityOption, ActivityOptionNumber, 
            ActivityOptionNumberValue, ActivityOptionSelection,
            Race, Ethnicity, RaceEthnicityValue
        } from '../activity/activity.service';
import { SnapClassic, SnapClassicService, zzSnapEdDeliverySite, zzSnapEdSessionTypes} from '../activity/snap-classic.service';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { Observable } from "rxjs/Observable";
import { ServicelogService, Servicelog, SnapDirectSessionType, SnapDirectAges, SnapDirectAudience, SnapDirectDeliverySite, SnapIndirectMethod, SnapIndirectReached, SnapPolicyAimed, SnapPolicyPartner } from "./servicelog.service";



@Component({
    selector: 'servicelog-form',
    templateUrl: 'servicelog-form.component.html',
    styleUrls: ['servicelog-form.component.scss'] 
})
export class ServicelogFormComponent implements OnInit{ 



    adminPrograms = [36, 37];
    isAdmin = false;

    @Input() activity:Servicelog = null;
    @Input() isNewDirect = false;
    @Input() isNewIndirect = false;
    @Input() isNewPolicy = false;
    @Input() isNewAdmin = false;


    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Servicelog>();

    loading = true;
    activityForm = null;

    optionArray:ActivityOption[];
    races:Race[];
    ethnicities:Ethnicity[];
    optionNumbers:ActivityOptionNumber[];
    initiatives:StrategicInitiative[];

    snapEligable = false;
    hasIndirect = false;
    hasDirect = false;

    raceEthnicityIndex = 0;

    genderRatio = 0.6;
    participantsNum = 0;
    femaleGhanged = false;

    // Snap Direct

    sessiontypes:Observable<SnapDirectSessionType[]>;
    snapdirectdeliverysite: Observable<SnapDirectDeliverySite[]>;

    // Snap Indirect

    snapindirectreached:Observable<SnapIndirectReached[]>

    // Snap Policy
    snapPolicy = false;

    snapFiscalYear17 = false;

    options:object;
    errorMessage:string;

    private myDatePickerOptions: IMyDpOptions = {
        // other options...
            dateFormat: 'mm/dd/yyyy',
            showTodayBtn: false,
            satHighlight: true,
            firstDayOfWeek: 'su'
        };

    constructor( 
        private fb: FormBuilder,
        private service: ServicelogService,
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

        // Snap Direct
        this.sessiontypes = this.service.sessiontypes();
        this.snapdirectdeliverysite = this.service.snapdirectdeliverysite();

        // Snap Indirect

        this.snapindirectreached = this.service.snapindirectreached();


    }

    ngOnInit(){
        this.programsService.listInitiatives().subscribe(
            i => this.initiatives = i,
            error =>  this.errorMessage = <any>error
        );
        this.populateOptions();


    }
    //Disable Snap Ed Checkbox for the 2017 fiscal year on date change
    onDateChanged(event: IMyDateModel) {
        if(event.date.year <= 2017 && event.date.month < 10){
            this.snapFiscalYear17 = true;
            this.activityForm.patchValue({isSnap:false});
            this.snapEligable = false;
        }else{
            this.snapFiscalYear17 = false;
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

    isPolicy(is:boolean){
        this.snapPolicy = is;
        this.activityForm.patchValue({isPolicy: is});
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
                isPolicy: false,
                snapAdmin: false,
                snapDirect: this.fb.group({
                        siteName: [""],
                        snapDirectDeliverySiteId: [""],
                        snapDirectSessionTypeId: [""],
                        snapDirectAgesAudienceValues:[[]]
                }),
                snapIndirect: this.fb.group({
                    snapIndirectMethodSelections:[[]],
                    snapIndirectReachedValues:[[]]
                }),
                snapPolicy: this.fb.group({
                    purpose: "",
                    result: "",
                    snapPolicyAimedSelections: [[]],
                    snapPolicyPartnerValue: [[]]
                }),
                snapCopies: [""]
                
            }, { validator: snapValidator }
        );
        this.myDatePickerOptions.disableSince = {year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() + 1};
        this.myDatePickerOptions.disableUntil = {year: 2017, month: 6, day: 30};
        this.myDatePickerOptions.editableDateField = false;
        this.myDatePickerOptions.showClearDateBtn = false;
        
        if(this.isNewAdmin){
            this.activityForm.patchValue({majorProgramId: this.adminPrograms[0], isSnap: true});
            this.checkIfAdminValue(this.adminPrograms[0]);
            this.snapEligable = true;
        }
        if(this.isNewDirect || this.isNewIndirect){
            this.activityForm.patchValue({isSnap: true});
            this.snapEligable = true;
        }
        if(this.isNewPolicy){
            this.activityForm.patchValue({isSnap: true, isPolicy:true});
            this.snapEligable = true;
            this.snapPolicy = true;
        }
        
        if(this.activity == null){
            var dtNow = new Date();
            
            if( dtNow.getFullYear() <= 2017 && dtNow.getMonth() < 9){
                this.snapFiscalYear17 = true;
            }else{
                this.snapFiscalYear17 = false;
            }
        }else{
            let date = new Date(this.activity.activityDate);
            if( date.getFullYear() <= 2017 && date.getMonth() < 9){
                this.snapFiscalYear17 = true;
                this.activityForm.patchValue({isSnap:false});
            }else{
                this.snapFiscalYear17 = false;
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
        if(this.activity.isPolicy){
            this.snapPolicy = true;
        }
        this.checkIfAdminValue(this.activity.majorProgramId);
        
    }

    

    onSubmit(){
        this.loading = true;
        var dateValue = this.activityForm.value.activityDate.date;
        var d = new Date(Date.UTC(dateValue.year, dateValue.month - 1, dateValue.day, 8, 5, 12));
        /*
        d.setMonth(dateValue.month - 1);
        d.setDate(dateValue.day);
        d.setFullYear(dateValue.year);
        */
        var val = this.activityForm.value;
        val.activityDate = d;

        var actOpt = val.activityOptionSelections;
        val.activityOptionSelections = actOpt.filter(x => x.selected).map(x => { return { activityOptionId: x.activityOptionId }; });
        
        if(!this.hasIndirect){
            val.snapIndirect = null;
        }
        if(this.snapPolicy || !(this.numParticipants() > 0)){
            val.snapDirect = null;
        }
        if(val.isSnap){
            if(this.isAdmin){
                val.snapDirect = null;
                val.snapIndirect = null;
                val.snapPolicy = null;
            }
        }else{
            this.isAdmin = false;
        }
        if(this.activity == null){
            this.service.add(val).subscribe(
                res => {
                    this.onFormSubmit.emit(res);
                    this.loading = false;
                }
            );
        }else{
            
            
            if(val.isSnap){
                if(val.snapAdmin){
                    val.snapDirect = null;
                    val.snapIndirect = null;
                    val.snapPolicy = null;
                }else{
                    if(this.hasIndirect){
                        for( let ind of val.snapIndirect.snapIndirectMethodSelections){
                            delete(ind.id);
                        }
                        for(let inval of val.snapIndirect.snapIndirectReachedValues){
                            delete(inval.id);
                        }
                    }
                    if( (val.male + val.female) > 0 ){
                        
                        if(val.isPolicy){
                            val.snapDirect = null;
                            for(let p of val.snapPolicy.snapPolicyPartnerValue){
                                delete(p.id);
                                p.snapPolicyPartner = null;
                            }
                            for( let s of val.snapPolicy.snapPolicyAimedSelections){
                                delete(s.id);
                            }
                        }else{
                            val.snapPolicy = null;
                            for(let ad of val.snapDirect.snapDirectAgesAudienceValues){
                                delete(ad.snapDirectAges);
                                delete(ad.snapDirectAudience);
                                delete(ad.id);
                            }
                        }
                    }
                }
            }
            this.service.update(this.activity.id, val).subscribe(
                res => {
                    this.onFormSubmit.emit(res);
                    this.loading = false;
                }
            );
            
        }
        

    }

    onCancel(){
        this.onFormCancel.emit();
    }

    

    /************************
      
      SNAP ED methods
    
     ***********************/

    checkIfAdmin(event){
        this.checkIfAdminValue(+event.target.value);
        /*
        if(this.adminPrograms.indexOf(+event.target.value) > -1){
            this.isAdmin = true;
            this.activityForm.patchValue({snapAdmin: true});
        }else{
            this.isAdmin = false;
            this.activityForm.patchValue({snapAdmin: false});
        }
        */
    }

    checkIfAdminValue(programId:number){
        if(this.adminPrograms.indexOf(programId) > -1){
            this.isAdmin = true;
            this.activityForm.patchValue({snapAdmin: true});
        }else{
            this.isAdmin = false;
            this.activityForm.patchValue({snapAdmin: false});
        }
    }

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
    if (isSnap.value === false) return null;
    let male = control.get('male');
    let female = control.get('female');

    if(parseInt(male.value) + parseInt(female.value) <= 0 ){
        return null;
    }

    if(control.get('snapAdmin').value) return null;

    if(control.get('isPolicy').value){

        let purpose = control.get('snapPolicy').get('purpose');
        let result = control.get('snapPolicy').get('result');
        if(!purpose.value && !result.value){
            return { nopurpose: true, noresult: true };
        }else if(!purpose.value){
            return { nopurpose: true };
        }else if(!result.value){
            return { noresult: true };
        }

        return null;
    }
    let site = control.get('snapDirect').get('snapDirectDeliverySiteId');
    let session = control.get('snapDirect').get('snapDirectSessionTypeId');
    let specificSite = control.get('snapDirect').get('siteName');

    if(!site.value && !session.value && !specificSite.value){
        return { nosite: true, nosession: true, nospecificSite: true };
    }else if(!site.value && !session.value){
        return { nosession: true, nosite: true };
    }else if(!site.value && !specificSite.value){
        return { nosite: true, nospecificSite: true };
    }else if(!session.value && !specificSite.value){
        return { nosession: true, nospecificSite: true };
    }else if(!session.value){
        return { nosession: true };
    }else if(!site.value){
        return { nosite: true };
    }else if(!specificSite.value){
        return { nospecificSite: true };
    }
    

    return null;
};