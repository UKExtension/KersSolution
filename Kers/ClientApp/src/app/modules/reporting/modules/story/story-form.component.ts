import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators, FormControl } from "@angular/forms";
import {Location} from '@angular/common';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { Observable } from "rxjs/Observable";
import {UserService, User} from '../user/user.service';
import {StoryService, Story, StoryImage, StoryOutcome} from './story.service';
import {PlansofworkService, PlanOfWork} from '../plansofwork/plansofwork.service';
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';



@Component({
    selector: 'story-form',
    templateUrl: 'story-form.component.html'
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

    outcome: Observable<StoryOutcome[]>;

    constructor( 
        private fb: FormBuilder,
        private programsService:ProgramsService,
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
                            story: ["", Validators.required],
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
                            story: ["", Validators.required],
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

    getFiscalYear(){
        if( this.story == null ){
            if( this.fiscalYear == null ){
                this.fiscalYearService.current("serviceLog").subscribe(
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
}

interface ImageResponse{

        link:string,
        imageId:number

    
};