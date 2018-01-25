import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import {    
            AffirmativeService, 
            AffirmativePlan,
            MakeupDiversityGroup,
            MakeupDiversity,
            MakeupValue,
            AdvisoryGroup,
            SummaryDiversity,
            SummaryValue
                                } from './affirmative.service';
import { FormBuilder, Validators, FormGroup,FormArray, FormControl }   from '@angular/forms';
import {Observable} from 'rxjs/Observable';

@Component({
    selector: 'affirmative-form',
    templateUrl: 'affirmative-form.component.html'
})
export class AffirmativeFormComponent implements OnInit{

    affirmativeForm = null;
    makeupValues = null;
    
    makeupLoaded = false;
    makeupIndex = 0;

    summaryLoaded = false;
    summaryIndex = 0;

    @Input() affirmativePlan:Observable<AffirmativePlan>;
    errorMessage: string;

    loading = false;
    options: object;
    

    makeupDiversityGroups: MakeupDiversityGroup[];
    advisoryGroups: AdvisoryGroup[];
    summaryDevirsity: SummaryDiversity[];

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private service: AffirmativeService,
        private fb: FormBuilder
    ){
        this.options = { 
            placeholderText: 'Your Description Here!',
            toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL'],
            toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
            quickInsertButtons: ['ul', 'ol', 'hr']
        }

        this.affirmativeForm = fb.group(
            {
              agents: [''],
              description: [''],
              goals: '',
              strategies: '',
              efforts: {value: '', disabled: true},
              success: {value: '', disabled: true},
              makeupValues: this.fb.array([]),
              summaryValues: this.fb.array([])
            }
        );

    }

   
    ngOnInit(){

        this.service.getMakeupDiversityGroups().subscribe(res => {
                this.makeupDiversityGroups = <MakeupDiversityGroup[]>res;
                this.service.getAdvisoryGroups().subscribe(res => {
                        this.advisoryGroups = res;
                        this.setMakeupValues();
                        this.service.getSummaryDiversity().subscribe(res => {
                                this.summaryDevirsity = res;
                                this.setSummaryValues();
                                
                                this.affirmativePlan.subscribe(
                                    res=>{
                                        this.affirmativeForm.patchValue(res);
                                    }
                                );
                                  
                            },
                            error =>  this.errorMessage = <any>error
                        );
                    },
                    error =>  this.errorMessage = <any>error
                );
            },
            error =>  this.errorMessage = <any>error
        );
        
    }

    mIndex(){
        if(this.makeupIndex == this.affirmativeForm.get("makeupValues").length){
            this.makeupIndex = 0
        }
        var val = ''+this.makeupIndex++
        return val;
    }

    sIndex(){
        if(this.summaryIndex == this.affirmativeForm.get("summaryValues").length){
            this.summaryIndex = 0
        }
        var val = ''+this.summaryIndex++
        return val;
    }

    setMakeupValues() {
        var values = [];
        var fb = this.fb;
        var makupGroup = this.makeupDiversityGroups;
        this.advisoryGroups.forEach(function(advsGroup){
            makupGroup.forEach(
                function(divGroup:MakeupDiversityGroup){
                    divGroup.types.forEach( function( mkType ){
                            var grp = fb.group(new MakeupValue( "", mkType.id, null, advsGroup.id, null));
                            values.push( grp );
                        }
                    );
                }
            );
        });
        this.affirmativeForm.setControl('makeupValues', this.fb.array(values));
        this.makeupLoaded = true;
    }

    setSummaryValues() {
        var values = [];
        var fb = this.fb;
        var devsty = this.summaryDevirsity;
        this.advisoryGroups.forEach(function(advsGroup){
            devsty.forEach(
                function(div:SummaryDiversity){
                    values.push(
                        fb.group( new SummaryValue("", div.id, null, advsGroup.id, null) )
                    );
                }
            );
        });
        this.affirmativeForm.setControl('summaryValues', this.fb.array(values));
        this.summaryLoaded = true;
    }


    onSubmit(){ 

        this.loading = true;

        this.service.add(this.affirmativeForm.value).
                subscribe(
                    res => {
                        this.onFormSubmit.emit();
                        this.loading = false;
                    }
            );


    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}