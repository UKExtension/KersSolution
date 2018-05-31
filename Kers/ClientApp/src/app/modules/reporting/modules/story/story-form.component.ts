import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormBuilder, Validators, FormControl } from "@angular/forms";
import {Location} from '@angular/common';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { Observable } from "rxjs/Observable";
import {UserService, User} from '../user/user.service';
import {StoryService, Story, StoryImage, StoryOutcome} from './story.service';
import {PlansofworkService, PlanOfWork} from '../plansofwork/plansofwork.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';



@Component({
    selector: 'story-form',
    templateUrl: 'story-form.component.html'
})
export class StoryFormComponent implements OnInit{ 

    @Input() story = null;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<Story>();
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
        private service: StoryService
    )   
    {
          
        

    }

    ngOnInit(){
        if( !this.fiscalYearSwitcher){
            this.plans = this.plansService.listPlans();
            this.programsService.listInitiatives().subscribe(
                i => this.initiatives = i,
                error =>  this.errorMessage = <any>error
            );
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

    fiscalYearSwitched( event:FiscalYear ){
        this.plans = this.plansService.listPlans(event.name);
        this.programsService.listInitiatives(event.name).subscribe(
            i => this.initiatives = i,
            error =>  this.errorMessage = <any>error
        );
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