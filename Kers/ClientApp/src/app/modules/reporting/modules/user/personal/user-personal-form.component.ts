import { Component,OnInit, Input, EventEmitter, Output } from '@angular/core';
import {    UserService,
            ExtensionPosition, 
            User,
            ReportingProfile,
            PersonalProfile,
            PlanningUnit,
            GeneralLocation,
            Interest,
            InterestProfile,
            UserSpecialty,
            Specialty,
            SocialConnectionType,
            SocialConnection 
        } from '../user.service';
import { FormBuilder, Validators, FormArray } from '@angular/forms';
import { Observable } from "rxjs";
import {Location} from '@angular/common';
import { PlanningunitService } from '../../planningunit/planningunit.service';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'user-personal-form',
    templateUrl: 'user-personal-form.component.html'
})
export class UserPersonalFormComponent implements OnInit { 

    @Input() userObservable:Observable<User> = null;

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<User>();

    user:User;

    interestTags=[];

    personalForm = null;
    public options: Object;
    public socialConnectionTypes:SocialConnectionType[];
    loading = false;
    editorOptionsLoaded = false;
    timezones: Observable<Object[]>;
    errorMessage: string;

    public imgObj: Object = {
        src: '/dist/assets/images/user.png'
    };

    constructor( 
        private userService: UserService,
        private unitService: PlanningunitService,
        private fb: FormBuilder,
        private location: Location,
        private http: HttpClient 
    )   
    {
         this.personalForm = this.fb.group({
              
              personalProfile: fb.group(
                {
                    firstName: ['', Validators.required],
                    lastName: ['', Validators.required],
                    professionalTitle: '',
                    officePhone: '',
                    mobilePhone: '',
                    officeAddress: '',
                    timeZoneId: '',
                    interests: [''],
                    socialConnections: fb.array([
                            this.initConnection()
                        ]),
                    bio: '',
                    uploadImageId: ''
                }
              )
            });

            
    }

    ngOnInit(){
       this.userService.socialConnections().subscribe(
           res => {
               this.socialConnectionTypes = res;
            
        
               this.userObservable.subscribe(
                    res => {
                            this.user = res;
                            if(this.user.personalProfile.socialConnections != null){
                                for(var i=1; i< this.user.personalProfile.socialConnections.length; i++){
                                        this.addConnection();
                                }
                            }
                            this.personalForm.patchValue(this.user);
                            var ins = [];
                            if(this.user.personalProfile.interests != null){
                                    this.user.personalProfile.interests.forEach(function(element){
                                            var el = {value: element.interest.name, display: element.interest.name};
                                            ins.push(el);
                                    });
                                    this.personalForm.controls.personalProfile.patchValue({ interests: ins })
                            } 
                            this.options = { 
                                placeholderText: 'About Me',
                                toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL','insertImage'],
                                toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat', '|','insertImage'],
                                toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
                                toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
                                imageUploadParams: { profileId: this.user.id },
                                imageUploadURL: this.location.prepareExternalUrl('/FroalaApi/UploadImage'),
                                fileUploadURL: this.location.prepareExternalUrl('/FroalaApi/UploadFile'),
                                imageManagerLoadURL: this.location.prepareExternalUrl('/FroalaApi/LoadImages'),
                                imageManagerDeleteURL: this.location.prepareExternalUrl('/FroalaApi/DeleteImage'),
                                imageManagerDeleteMethod: "POST"
                            }
                            this.editorOptionsLoaded = true;              
                        },
                        error => this.errorMessage = <any> error
                )



            },
           error => this.errorMessage = <any> error
       );
       this.timezones = this.unitService.timezones();
    }

    public requestAutocompleteItems = (text: string):Observable<Object> => {
        const url = '/api/User/tags?q=' + text;
        return this.http
            .get(this.location.prepareExternalUrl(url));
    };


    initConnection(){
        return this.fb.group({
            socialConnectionTypeId: [''],
            identifier: ['']
        });
    }

    addConnection() {
        // add address to the list
        const control = <FormArray>this.personalForm.controls.personalProfile.controls['socialConnections'];
        control.push(this.initConnection());
    }

    removeConnection(i: number) {
        const control = <FormArray>this.personalForm.controls.personalProfile.controls['socialConnections'];
        control.removeAt(i);
    }

    onSubmit(){
        var val = this.personalForm.value;
        
        this.loading = true;

        var ints = []
        
        if(val.personalProfile.interests != null && val.personalProfile.interests.length > 0){
            val.personalProfile.interests.forEach(function(element){
                var i = new InterestProfile(null, new Interest( null, element.value), null);
                ints.push(i);
            });
        }
        
        val.personalProfile.interests = ints; 
        
        if(this.user){
            this.userService.update(this.user.id, <User> val  ).subscribe(
                res => {
                    this.loading = false;
                    this.onFormSubmit.emit(<User> res);
                    
                },
                error => this.errorMessage = <any>error
            )
        }
    }

    OnCancel(){
        this.onFormCancel.emit();
    } 
}