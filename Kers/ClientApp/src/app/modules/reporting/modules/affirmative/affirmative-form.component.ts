import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import {    
            AffirmativeService, 
            AffirmativePlan,
            MakeupDiversityGroup,
            MakeupDiversity,
            MakeupValue,
            AdvisoryGroup,
            SummaryDiversity,
            SummaryValue,
            MakeupValueForm
                                } from './affirmative.service';
import { FormBuilder, Validators, FormGroup,FormArray, FormControl }   from '@angular/forms';
import {Observable} from 'rxjs';

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
    @Input() isReport:boolean = false;
    errorMessage: string;

    loading = false;
    options: object;

    plan:AffirmativePlan;
    condition = false;
    planCondition = true;
    

    makeupDiversityGroups: MakeupDiversityGroup[];
    advisoryGroups: AdvisoryGroup[];
    summaryDevirsity: SummaryDiversity[];

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private service: AffirmativeService,
        private fb: FormBuilder
    ){
        

        

    }

   
    ngOnInit(){
        this.loading = true;
        var isRprt = this.isReport;
        this.options = { 
            placeholderText: 'Your Description Here!',
            toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL'],
            toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
            quickInsertButtons: ['ul', 'ol', 'hr'],
            events : {
                'froalaEditor.initialized' : function(e, editor) {
                    if(isRprt){
                        editor.edit.off();
                    }
                }
              }
        }
        this.affirmativeForm = this.fb.group(
            {
              agents: [{value: '', disabled: isRprt}],
              description: [{value: '', disabled: isRprt}],
              goals: [{value: '', disabled: isRprt}],
              strategies: [{value: '', disabled: isRprt}],
              efforts: {value: '', disabled: !isRprt},
              success: {value: '', disabled: !isRprt},
              makeupValues: this.fb.array([]),
              summaryValues: this.fb.array([]),
              affirmativeActionPlanId: 0
            }
        );
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
                                        if(res != null){
                                            this.plan = res;
                                            this.affirmativeForm.patchValue(res);
                                        }
                                        this.loading = false;
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
        this.condition = this.isReport;
        this.planCondition = !this.isReport;
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
        var isRprt = this.isReport;
        var makupGroup = this.makeupDiversityGroups;
        this.advisoryGroups.forEach(function(advsGroup){
            makupGroup.forEach(
                function(divGroup:MakeupDiversityGroup){
                    divGroup.types.forEach( function( mkType ){
                            var grp = fb.group(new MakeupValueForm( {value:"", disabled:isRprt}, mkType.id, null, advsGroup.id, null));
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
        var isRprt = this.isReport;
        this.advisoryGroups.forEach(function(advsGroup){
            devsty.forEach(
                function(div:SummaryDiversity){
                    values.push(
                        fb.group( new SummaryValue({value:"", disabled:!isRprt}, div.id, null, advsGroup.id, null) )
                    );
                }
            );
        });
        this.affirmativeForm.setControl('summaryValues', this.fb.array(values));
        this.summaryLoaded = true;
    }


    onSubmit(){ 

        this.loading = true;

        this.service.add(this.affirmativeForm.getRawValue()).
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