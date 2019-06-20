import { Component } from '@angular/core';
import {ReportingService} from '../../components/reporting/reporting.service';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import {IndicatorsService, Indicator} from './indicators.service';
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';

@Component({
  template: `
<div class="alert alert-danger alert-dismissible fade in" role="alert" *ngIf="errorMessage">
    <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span>
    </button>
    <strong>Error: </strong> {{errorMessage}}
</div>

    These numbers are to be kept up to date PER INDIVIDUAL (YOU) - NOT THE COUNTY.<br>
Simply update the numbers as needed throughout the fiscal year.<br>
<strong>ENTER WHOLE NUMBERS ONLY.</strong><br>

<div class="alert alert-success alert-dismissible fade in" role="alert" *ngIf="dataSubmitted">
    <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span>
    </button>
    <strong>Indicator data for <em>{{selectedProgram.name}}</em> has been submitted </strong><br>
To enter/adjust data for another Major Program, just select the Major Program from the dropdown list bellow.
</div>
<br>
<loading *ngIf="initiatives == null"></loading>
<div *ngIf="initiatives != null">
    Select the Major Program from the dropdown list to access/enter your Program Indicator numbers:<br>
    <select name="mp3" id="mp3" class="form-control col-md-7 col-xs-12" (change)="onChange($event.target.value)">
        <option value="">--- select ---</option>
        <optgroup  *ngFor="let initiative of initiatives" label="{{initiative.name}}">
            <option *ngFor="let program of initiative.majorPrograms" [value]="program.id">{{program.name}} ({{program.pacCode}})</option>
        </optgroup>
    </select>
    <div *ngIf="selectedProgram"><br><br><br>
        <h2>{{selectedProgram.name}}</h2><br>

        <div class="row" *ngIf="selectedIndicators != null && selectedIndicators.length > 0">

            <form class="form-horizontal form-label-left col-sm-12" novalidate (ngSubmit)="onSubmit()" [formGroup]="indicatorsForm">
                <loading *ngIf="loading"></loading>
                
                <div formArrayName="indicatorsGroup" *ngIf="!loading">
                    
                        <div *ngFor="let indicator of indicatorsForm.controls.indicatorsGroup.controls; let i=index" [formGroupName]="i" class="form-group">
                            <div class="col-xs-3">
                                
                                    <input class="form-control" type="number" id="firstName" formControlName="value" />
                                
                            </div>
                            <div class="col-xs-9" [innerHTML]="selectedIndicators[i].question"></div>
                        
                    </div>
                </div> 
                <div class="ln_solid"></div>
                <div class="form-group">
                    <div class="col-xs-12">
                        <button type="submit" [disabled]="indicatorsForm.invalid"  class="btn btn-success">Submit</button>
                    </div>
                </div>
                    
            </form>

        </div>
        <div *ngIf="selectedIndicators != null && selectedIndicators.length == 0">There are no indicators available for selected Major Program</div>
    </div>
</div>
  `
})
export class IndicatorsHomeComponent { 
    
    initiatives:StrategicInitiative[];
    programs: MajorProgram[];
    selectedProgram: MajorProgram;
    selectedIndicators: Indicator[];
    fiscalYear:FiscalYear;
    loading = false;

    errorMessage:string;
    indicatorsForm = null;

    dataSubmitted = false;
    
//<fiscal-year-switcher [initially]="current" (onSwitched)="fiscalYearSwitched($event)"></fiscal-year-switcher>
    constructor( 
        private reportingService: ReportingService, 
        private fb: FormBuilder,
        private programsService:ProgramsService,
        private indicatorsService:IndicatorsService,
        private fiscalYearService:FiscalyearService
    )   
    {
        this.indicatorsForm = this.fb.group({
            indicatorsGroup: fb.array([])
        });

    }

    ngOnInit(){
        this.fiscalYearService.current("serviceLog", true).subscribe(
            res => {
                var prgrms = [];
                this.initiatives = null;
                this.selectedProgram = null;
                this.fiscalYear = res;
                this.defaultTitle();
                this.programsService.listInitiatives(this.fiscalYear.name).subscribe(
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
        )
        
    }
    onChange(programId) {
        this.dataSubmitted=false;
        if(programId != ""){
            this.selectedProgram = this.programs.filter(p=>p.id == programId)[0];
            this.indicatorsService.listIndicators(this.selectedProgram).subscribe(
                res=>{
                    this.selectedIndicators = <Indicator[]>res;
                    this.updateForm();
                    this.indicatorsService.indicatorValues(this.selectedProgram).subscribe(
                        r=>{
                            this.indicatorsForm.patchValue({indicatorsGroup:r})
                        },
                        err=>this.errorMessage = <any> err
                    )
                },
                err=>this.errorMessage = <any> err
            )
        }else{
            this.selectedProgram = null;
            this.selectedIndicators = [];
        }
        
    }

    updateForm(){
        var indicatorsFormElements = [];
        var thisObject = this;
        this.selectedIndicators.forEach(function(element){
            indicatorsFormElements.push( thisObject.initValue(element.id) );
        });
        this.indicatorsForm.setControl('indicatorsGroup',this.fb.array(indicatorsFormElements));
    }

    initValue(indicatorId:number, value=0){
        return this.fb.group({
            programIndicatorId: [indicatorId],
            value: [value]
        });
    }

    onSubmit(){
        this.loading = true;
        this.indicatorsService.
            updateValues(this.selectedProgram, this.indicatorsForm.value.indicatorsGroup).
            subscribe(
                res=>{
                    this.dataSubmitted=true;
                    this.loading = false;
                },
                err=>this.errorMessage = <any> err
        );
    }

    

    defaultTitle(){
        this.reportingService.setTitle("Program Indicators for FY"+this.fiscalYear.name);
    }
}