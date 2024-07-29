import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators, FormControl } from "@angular/forms";
import {Location} from '@angular/common';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { Observable } from "rxjs";
import {UserService, User} from '../user/user.service';
import {StoryService, Story, StoryImage, StoryOutcome, StoryAudienceType, StoryAudienceConnection} from './story.service';
import {PlansofworkService, PlanOfWork} from '../plansofwork/plansofwork.service';
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';
import { Indicator, IndicatorsService } from '../indicators/indicators.service';



@Component({
    selector: 'story-form',
    templateUrl: 'story-form.component.html',
    styles: [`
    ng-select.ng-invalid{
        border: 1px solid red;
    }
    `]
})
export class StoryFormComponent implements OnInit{ 

    @Input() story = null;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Story>();
    @Input() fiscalYear:FiscalYear | null = null;
    @Input() fiscalYearSwitcher = false;

    loading = true;
    storyForm = null;
    options:object;
    storyImages: StoryImage[] = [];
    errorMessage:string;
    storyId = 0;
    currentUser:User;
    
    editorOptionsLoaded = false;
    initiatives:StrategicInitiative[];
    plans: Observable<PlanOfWork[]>;
    audience: Observable<StoryAudienceType[]>;
    otherAudienceSelected:boolean = false;

    indicators:Indicator[];

    outcome: Observable<StoryOutcome[]>;

    audienceTypes = [];




    constructor( 
        private fb: FormBuilder,
        private programsService:ProgramsService,
        private indicatorsService: IndicatorsService,
        private location: Location,
        private userService:UserService,
        private plansService: PlansofworkService,
        private service: StoryService,
        private fiscalYearService: FiscalyearService
    )   
    {
          
        

    }

    ngOnInit(){
        if( !this.fiscalYearSwitcher){
            this.getFiscalYear();
        }
        this.service.audiencetype().subscribe(
            res => {
                var auds = <StoryAudienceType[]> res;
                var au = Array<any>();
                auds.forEach(function(element){
                    au.push(
                        {value: element.id, label: element.name, isItOther: element.isItOther}
                    );
                });
                this.audienceTypes = au;



                if(this.story != null){
                    if(this.story.storyAudienceConnections != undefined && this.story.storyAudienceConnections.length > 0){
                        var aud = [];
                        var isOther = false;
                        for( var ad of this.story.storyAudienceConnections){
                            if(ad.storyAudienceType != undefined && ad.storyAudienceType.name != undefined ){
                                aud.push( {value: ad.storyAudienceType.id, label: ad.storyAudienceType.name} );
                                if(ad.storyAudienceType.name == "Other") isOther = true;
                            }else{
                                var at = auds.filter( t => t.id == ad.storyAudienceTypeId);
                                aud.push({value: at[0].id, label: at[0].name}  );
                                if(at[0].name == "Other") isOther = true;
                            }   
                        }
                        this.storyForm.patchValue({storyAudienceConnections:aud});
                        if(isOther) this.otherAudienceSelected = true;
                    }
                }




            }
        );
        this.outcome = this.service.outcome();
        this.userService.current().subscribe(
            res => {
                this.currentUser = <User> res;
                var thisObject = this;
                this.options = { 
                        placeholderText: 'Your Success Story Here!',
                        toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL', 'insertImage', 'insertVideo'],
                        toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'insertImage'],
                        toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'paragraphFormat', '|', 'insertImage'],
                        toolbarButtonsXS: ['bold', 'italic', 'paragraphFormat', '|', 'insertImage'],
                        quickInsertButtons: ['ul', 'ol', 'hr'],
                        imageUploadParams: { profileId: this.currentUser.id },
                        videoInsertButtons: ['videoBack', '|', 'videoByURL', 'videoEmbed'],
                        imageUploadURL: this.location.prepareExternalUrl('/FroalaApi/UploadImage'),
                        events: {
                            'froalaEditor.image.uploaded':function (e, editor, response){
                                var o = <ImageResponse>JSON.parse(response);
                                var im = <StoryImage>{uploadImageId: o.imageId}
                                thisObject.storyImages.push(im);
                            }
                        }    
                }
                if(this.currentUser.extensionPosition.code == 'AGENT'){
                    this.storyForm = this.fb.group(
                        {
                            title: ["", Validators.required],
                            storyAudienceConnections: [[],Validators.required],
                            audienceOther: "",
                            reach:["", this.isPositiveInt],
                            story: ["Describe the Issue or Situation.<br><br>Describe the Outreach or Educational Program Response (and Partners, if applicable).<br><br>Provide the Number and Description(s) of Participants/Target Audience.<br><br>Provide a Statement of Outcomes or Program Impact. <em>Please note that the outcomes statement must use evaluation data to describe the change(s) that occurred in individuals, groups, families, businesses, or in the community because of the program/outreach.</em>", Validators.required],
                            isSnap: [false, Validators.required],
                            majorProgramId: ["", Validators.required],
                            planOfWorkId: ["", Validators.required],
                            storyOutcomeId: ["", Validators.required]
                        }
                    );
                }else{
                    this.storyForm = this.fb.group(
                        {
                            title: ["", Validators.required],
                            storyAudienceConnections: [[],Validators.required],
                            audienceOther: "",
                            reach:["", this.isPositiveInt],
                            story: ["Describe the Issue or Situation.<br><br>Describe the Outreach or Educational Program Response (and Partners, if applicable).<br><br>Provide the Number and Description(s) of Participants/Target Audience.<br><br>Provide a Statement of Outcomes or Program Impact. <em>Please note that the outcomes statement must use evaluation data to describe the change(s) that occurred in individuals, groups, families, businesses, or in the community because of the program/outreach.</em>", Validators.required],
                            isSnap: [false, Validators.required],
                            majorProgramId: ["", Validators.required],
                            planOfWorkId: [""],
                            storyOutcomeId: ["", Validators.required]
                        }
                    );
                }
                if(this.story != null){
                    this.storyImages = this.story.storyImages;
                    this.storyForm.patchValue(this.story);
                }
                this.loading = false;
                this.editorOptionsLoaded = true;
                
            },
            err => this.errorMessage = <any> err
        )
        
    }
    AudienceChanged(event:[]){
        var a = event.filter( a => a["isItOther"] == true);
        if(a.length > 0){
            this.otherAudienceSelected = true;
        }else{
            this.otherAudienceSelected = false;
        }
    }

    getFiscalYear(){
        if( this.story == null ){
            if( this.fiscalYear == null ){
                this.fiscalYearService.current("serviceLog", true).subscribe(
                    res => {
                        this.fiscalYear =<FiscalYear> res;
                        this.getInitiatives();
                        this.plans = this.plansService.listPlans(this.fiscalYear.name);
                    },
                    error => this.errorMessage = <any>error
                )
            }else{
                this.getInitiatives();
                this.plans = this.plansService.listPlans(this.fiscalYear.name);
            }
        }else{
            this.fiscalYearService.byType("serviceLog").subscribe(
                res => {
                    this.fiscalYear = this.getFiscalYearByStory();
                    this.getInitiatives();
                    this.plans = this.plansService.listPlans(this.fiscalYear.name);
                }
            )
            
        }
        
    }

    getInitiatives(){
        this.programsService.listInitiatives(this.fiscalYear.name).subscribe(
            i => this.initiatives = i,
            error =>  this.errorMessage = <any>error
        );
    }

    getFiscalYearByStory():FiscalYear{
        var year = this.story.majorProgram.strategicInitiative.fiscalYear;

        year.start = new Date(year.start);
        year.end = new Date(year.end);

        return year;
    }

    ProgramChanged(event){
        var mpId = this.storyForm.value.majorProgramId;
        
        this.indicatorsService.indicatorsforprogram(mpId).subscribe(
            res => {
                this.indicators = res;
            }
        )
    }



    fiscalYearSwitched( event:FiscalYear ){
        this.fiscalYear = event;
        this.initiatives = null;
        this.plans = this.plansService.listPlans(event.name);
        this.getInitiatives();
    }
        


    onSubmit(){
        this.loading = true;
        var val = this.storyForm.value;
        val.storyImages = this.storyImages;
        //remove id-s from story images as it will create duplicate key error otherwise
        for( var im of val.storyImages){
            delete im.id;
        }
        var connections = val.storyAudienceConnections;
        var StoryAudienceConnections = [];
        for( var cn of connections){
            StoryAudienceConnections.push({storyAudienceTypeId:cn["value"]});
        }
        val.storyAudienceConnections = StoryAudienceConnections;        
        if(this.story == null){
            this.service.add(val).subscribe(
                res => {
                    this.loading = false;
                    this.onFormSubmit.emit(<Story>res);
                },
                err => this.errorMessage = <any>err
            );
        }else{
            this.service.update(this.story.id, val).subscribe(
                res => {
                    this.loading = false;
                    this.onFormSubmit.emit(<Story>res);
                },
                err => this.errorMessage = <any>err
            );
        }
 
    }

    onCancel(){
        this.onFormCancel.emit();
    }


    //validator
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