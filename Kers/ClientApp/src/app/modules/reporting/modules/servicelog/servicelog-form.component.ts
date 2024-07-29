import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators, FormControl, AbstractControl } from "@angular/forms";
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import {    ActivityOption, ActivityOptionNumber, 
            Race, Ethnicity, ActivityImage
        } from '../activity/activity.service';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { Observable } from 'rxjs';
import { ServicelogService, Servicelog, SnapDirectSessionType, SnapDirectDeliverySite, SnapIndirectReached, SnapDirectSessionLength, SnapIndirectAudienceTargeted } from "./servicelog.service";
import { FiscalyearService, FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import {Location} from '@angular/common';
import { UserService, User } from '../user/user.service';
import { SignupService } from '../signup/signup.service';
import { ExtensionEventLocation } from '../events/extension-event';
import {ExtensionEventLocationConnection} from '../events/location/location.service';


@Component({
    selector: 'servicelog-form',
    templateUrl: 'servicelog-form.component.html',
    styleUrls: ['servicelog-form.component.scss'] 
})
export class ServicelogFormComponent implements OnInit{ 


    @Input() activity:Servicelog = null;
    @Input() activityId:number;
    @Input() activityDate:Date;
    @Input() isNewDirect = false;
    @Input() isNewIndirect = false;
    @Input() isNewPolicy = false;
    @Input() isNewAdmin = false;


    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Servicelog>();

    logLoading = true;
    activityForm = null;
    hasAttendance:Observable<boolean>;
    attendiesOpen = false;
    optionArray:ActivityOption[];
    races:Race[];
    ethnicities:Ethnicity[];
    optionNumbers:ActivityOptionNumber[];
    initiatives:StrategicInitiative[];
    activityImages:ActivityImage[] = [];

    fiscalYear:FiscalYear;
    currentUser:User;



    snapDirectLoc:ExtensionEventLocation;
    snapDirectLocId:number;
    snapDirectlocationBrowser:boolean = true;

    snapEligable = false;
    hasIndirect = false;
    hasDirect = false;

    //adminPrograms = [36, 37, 236, 206];
    adminProgramsCodes = [7001, 7002];
    isAdmin = false;

    raceEthnicityIndex = 0;

    genderRatio = 0.6;
    participantsNum = 0;
    femaleGhanged = false;

    // Snap Direct

    sessiontypes:Observable<SnapDirectSessionType[]>;
    sessionlengths:Observable<SnapDirectSessionLength[]>;
    snapdirectdeliverysite: Observable<SnapDirectDeliverySite[]>;

    // Snap Indirect

    snapindirectreached:Observable<SnapIndirectReached[]>
    snapIndirectAudienceTargeted: Observable<SnapIndirectAudienceTargeted[]>;

    // Snap Policy
    snapPolicy = false;

    isPastSnapFiscalYear = false;
    previousFiscalYear = 2024;

    signupOpen = false;

    options:object;
    errorMessage:string;

    public cond = false;
    public condition = false;

    isToday = (someDate:Date) => {
        const today = new Date()
        return someDate.getDate() == today.getDate() &&
          someDate.getMonth() == today.getMonth() &&
          someDate.getFullYear() == today.getFullYear()
      }

    private myDatePickerOptions: IAngularMyDpOptions = {
        // other options...
            dateFormat: 'mm/dd/yyyy',
            satHighlight: true,
            firstDayOfWeek: 'su'
        };

    constructor( 
        private fb: FormBuilder,
        private service: ServicelogService,
        private programsService:ProgramsService,
        private fiscalYearService: FiscalyearService,
        private location: Location,
        private userService:UserService,
        private signupService:SignupService
    )   
    {
          
        

        // Snap Direct
        this.sessiontypes = this.service.sessiontypes();
        this.sessionlengths = this.service.sessionlengths();
        this.snapdirectdeliverysite = this.service.snapdirectdeliverysite();

        // Snap Indirect

        this.snapindirectreached = this.service.snapindirectreached();
        this.snapIndirectAudienceTargeted = this.service.snapIndirectAudienceTargeted();
        


    }

    locationSelected(evnt:ExtensionEventLocationConnection){
        this.snapDirectLoc = evnt.extensionEventLocation;
        this.snapDirectLocId = evnt.extensionEventLocationId;
        this.activityForm.controls.snapDirect.patchValue({еxtensionEventLocation:this.snapDirectLoc});
        this.snapDirectlocationBrowser = false;
    }

    locationToBeChanged(){
        this.activityForm.controls.snapDirect.patchValue({еxtensionEventLocation:null});
        this.snapDirectlocationBrowser = true;
        this.snapDirectLoc = null;
    }

    ngOnInit(){
        this.userService.current().subscribe(
            res => {
                this.currentUser = <User> res;
                var thisObject = this;
                this.options = { 
                    placeholderText: 'Your Description Here!',
                    toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL', 'insertImage'],
                    toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'insertImage'],
                    toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'insertImage'],
                    toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic', '|', 'insertImage'],
                    quickInsertButtons: ['ul', 'ol', 'hr'], 
                    imageUploadParams: { profileId: this.currentUser.id },
                    imageUploadURL: this.location.prepareExternalUrl('/FroalaApi/UploadImage'),  
                    events: {
                        'froalaEditor.image.uploaded':function (e, editor, response){
                            var o = <ImageResponse>JSON.parse(response);
                            var im = <ActivityImage>{uploadImageId: o.imageId}
                            thisObject.activityImages.push(im);
                        },
                        'froalaEditor.image.beforeRemove':function (image){
                        }
                    } 
                }
            }
        );
        if( this.activity != null){
            this.getFiscalYear(new Date(this.activity.activityDate));
            this.hasAttendance = this.signupService.hasAttendance(this.activity.activityId);
        }else if( this.activityDate != null ){
            this.getFiscalYear( this.activityDate );
        }else{
            this.getFiscalYear( new Date() );
        }
        this.cond = this.service.cond;
        this.condition = this.service.condition;
        
        this.populateOptions();


    }

    getFiscalYear(date:Date){
        var dt = new Date(date);
        this.fiscalYearService.forDate(  date ).subscribe(
            res => {
                this.fiscalYear = <FiscalYear> res;
                this.getInitiatives(this.fiscalYear);
            }
        )
    }

    getInitiatives( fy:FiscalYear ){
        
        this.programsService.listInitiatives( fy.name ).subscribe(
            i => {
                this.initiatives = i;
                
                if(this.activity != null){
                    //this.patch();
                    this.checkIfAdminValue(this.activity.majorProgramId);
                }else{
                    // Checks if it is an admin major program on a new record
                    if(this.isNewAdmin){
                        //Find Major Program Id of the Admin Functions Pac Code
                        var program:MajorProgram;
                        for( var initiative of this.initiatives ){
                            var adm = initiative.majorPrograms.filter( m => m.pacCode == this.adminProgramsCodes[1] );
                            if( adm.length > 0 ){
                                program = adm[0];
                                break;
                            }
                        }
                        if(program != undefined){
                            this.activityForm.patchValue({majorProgramId: program.id, isSnap: true});
                            this.checkIfAdminValue(program.id);
                            this.snapEligable = true;
                        }
                    }
                }
            },
            error =>  this.errorMessage = <any>error
        );
    }




    //Disable Snap Ed Checkbox for the previous fiscal year on date change
    onDateChanged(event: IMyDateModel) {
        var date = event.singleDate.jsDate;
        if(date.getFullYear() <= this.previousFiscalYear && date.getMonth() < 10){
            this.isPastSnapFiscalYear = true;
            this.activityForm.patchValue({isSnap:false});
            this.snapEligable = false;
        }else{
            this.isPastSnapFiscalYear = false;
        }
        if(  this.fiscalYear.start > date || this.fiscalYear.end < date   ){
            this.getFiscalYear( date );
            if(this.activityForm != undefined) this.activityForm.patchValue({majorProgramId: "" });
            this.isAdmin = false;
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
                        if(this.activity != null){
                            this.patch();
                        }
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
    raceEthnicityValue( raceId:number, ethnctId:number){
        var value = 0;
        if( this.activity != null){
            let val = this.activity.raceEthnicityValues.filter( v => v.raceId == raceId && v.ethnicityId == ethnctId);
            if( val.length != 0 ){
                value = val[0].amount;
            }   
        }
        return value;
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
                    isRange: false, singleDate: {jsDate: date}

                    /* 
                                date: {
                                    year: date.getFullYear(),
                                    month: date.getMonth() + 1,
                                    day: date.getDate()} */
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
                        еxtensionEventLocation: [""],
                        snapDirectSessionTypeId: [""],
                        snapDirectSessionLengthId: [""],
                        snapDirectAgesAudienceValues:[[]]
                }),
                snapIndirect: this.fb.group({
                    snapIndirectMethodSelections:[[]],
                    snapIndirectReachedValues:[[]],
                    snapIndirectAudienceTargetedId:[""]
                }),
                snapPolicy: this.fb.group({
                    purpose: "",
                    result: "",
                    snapPolicyAimedSelections: [[]],
                    snapPolicyPartnerValue: [[]]
                }),
                snapCopies: [""],
                snapCopiesBW: [""]
                
            }, { validator: snapValidator }
        );
        this.myDatePickerOptions.disableSince = {year: date.getFullYear(), month: date.getMonth() + 1, day: date.getDate() + 1};
        this.myDatePickerOptions.disableUntil = {year: 2024, month: 6, day: 30};
        
        
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
            
            if( dtNow.getFullYear() <= this.previousFiscalYear && dtNow.getMonth() < 9){
                this.isPastSnapFiscalYear = true;
            }else{
                this.isPastSnapFiscalYear = false;
            }



            if(this.activityDate != null){
                this.activityForm.patchValue({activityDate: {
                        isRange: false, singleDate: {jsDate: this.activityDate}
                    }});
            }

            this.logLoading = false;

        }else{
            let date = new Date(this.activity.activityDate);
            if( date.getFullYear() <= this.previousFiscalYear && date.getMonth() < 9){
                this.isPastSnapFiscalYear = true;
                this.activityForm.patchValue({isSnap:false});
            }else{
                this.isPastSnapFiscalYear = false;
            }
            
        }

        
    }

    patch(){
        if(this.isAdmin){
            this.activityForm.patchValue({snapAdmin: true});
        }else{
            this.activityForm.patchValue({snapAdmin: false});
        }
        let date = new Date(this.activity.activityDate);
        this.activity.activityDate = date;
        this.activityForm.get('female').markAsDirty();
        let raceEthnArray = [];
        for(let rc of this.races){
            for(let et of this.ethnicities){
                var vl = 0;
                var filtered = this.activity.raceEthnicityValues.filter( a =>  a.raceId == rc.id && a.ethnicityId == et.id);
                if(filtered.length > 0 ){
                    vl = filtered[0].amount;
                }
                raceEthnArray.push({
                    amount: vl,
                    raceId: rc.id,
                    ethnicityId: et.id
                });                
            }
        }
        this.activity.raceEthnicityValues = raceEthnArray;
        this.activityForm.patchValue(this.activity);
        let opArray = [];
        for( let option of this.optionArray){
            opArray.push({
                selected: this.activity.activityOptionSelections.filter(s => s.activityOptionId == option.id).length != 0,
                activityOptionId: option.id
            });
        }
        
        this.activityForm.patchValue({activityOptionSelections:opArray});
        this.activityForm.patchValue({activityDate: 
            {
                isRange: false, singleDate: {jsDate: date}
            }});
        if(this.activity.isSnap){
            this.snapEligable = true;
            if(this.activity.snapDirect){
                if(this.activity.snapDirect.extensionEventLocation){
                    this.snapDirectLoc = this.activity.snapDirect.extensionEventLocation;
                    this.snapDirectLocId = this.activity.snapDirect.еxtensionEventLocationId;
                    this.activityForm.controls.snapDirect.patchValue({еxtensionEventLocation:this.snapDirectLoc});
                    this.snapDirectlocationBrowser = false;
                }
            }
        }
        
        if(this.activity.activityOptionNumbers.filter(n => n.activityOptionNumberId == 3)[0].value > 0){
            this.hasIndirect = true;
        }
        if(this.activity.isPolicy){
            this.snapPolicy = true;
        }
        
        
        this.logLoading = false;
    }

    

    onSubmit(){
        this.logLoading = true;
        var dateValue = this.activityForm.value.activityDate.singleDate.jsDate;
        var d = new Date(Date.UTC(dateValue.getFullYear(), dateValue.getMonth(), dateValue.getDate(), 8, 5, 12));
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
                val.snapAdmin = true;
                val.snapDirect = null;
                val.snapIndirect = null;
                val.snapPolicy = null;
            }else{
                val.snapAdmin = false;
            }
            if(val.snapDirect != undefined){
                val.snapDirect.extensionEventLocation = this.snapDirectLoc;
            }
        }else{
            this.isAdmin = false;
            val.snapAdmin = false;
        }
        val.activityImages = this.activityImages;
        if(this.activity == null){
            this.service.add(val).subscribe(
                res => {
                    this.onFormSubmit.emit(res);
                    this.logLoading = false;
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
                    this.logLoading = false;
                }
            );
            
        }
        

    }

    onCancel(){
        this.onFormCancel.emit();
    }
    setCond(state:boolean){
        this.cond = state;
        this.service.cond = state;
    }
    setCondition( state: boolean ){
        this.condition = state;
        this.service.condition = state;
    }

    

    /************************
      
      SNAP ED methods
    
     ***********************/

    checkIfAdmin(event){
        this.checkIfAdminValue(+event.target.value);
    }

    checkIfAdminValue(programId:number){

        var program:MajorProgram;
        for( var initiative of this.initiatives ){
            var adm = initiative.majorPrograms.filter( m => m.id == programId );
            if( adm.length > 0 ){
                program = adm[0];
                break;
            }
        }
        
        if(program != undefined){
            if(this.adminProgramsCodes.indexOf(program.pacCode) > -1 ){   
                this.isAdmin = true;
                //this.activityForm.patchValue({snapAdmin: true});
            }else{
                this.isAdmin = false;
                //this.activityForm.patchValue({snapAdmin: false});
            }
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

     displayAttendiesButton(){
         var display = false;
         if( this.activity != null){
            display = this.isToday(this.activity.activityDate);
         }
         return display;
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
interface ImageResponse{

    link:string,
    imageId:number


};



export const snapValidator = (control: AbstractControl): {[key: string]: boolean} => {
    const isSnap = control.get('isSnap');
    if (isSnap.value === false) return null;
    let male = control.get('male');
    let female = control.get('female');
    var error = {};
    var hasError = false;
    if(control.get('activityOptionNumbers').value.filter(n => n.activityOptionNumberId == 3)[0].value > 0) {
        if(control.get('snapAdmin').value) return null;
        let audienceTargeted = control.get('snapIndirect').get('snapIndirectAudienceTargetedId');
        if(audienceTargeted.value == ""){
            error["audienceTargeted"] = true;
            hasError = true;
        }
    }
   
    if(parseInt(male.value) + parseInt(female.value) > 0 ){
       

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
        let length = control.get('snapDirect').get('snapDirectSessionLengthId');
        let specificSite = control.get('snapDirect').get('еxtensionEventLocation');

        if(!site.value){
            error["nosite"] = true;
            hasError = true;
        }
        if(!session.value){
            error["nosession"] = true;
            hasError = true;
        }
        if(!length.value){
            error["nolength"] = true;
            hasError = true;
        }

        if(!specificSite.value){
            error["nospecificSite"] = true;
            hasError = true;
        }
    }
    
    if(hasError) return error;

    return null;
};