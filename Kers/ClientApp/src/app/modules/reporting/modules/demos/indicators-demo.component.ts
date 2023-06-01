import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ProgramsService, StrategicInitiative } from '../admin/programs/programs.service';
import { Indicator, IndicatorsService } from '../indicators/indicators.service';
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';

@Component({
  selector: 'app-indicators-demo',
  template: `
    




  <div class="row" *ngIf="!loading">
  <div><reporting-display-help id="4"></reporting-display-help></div>
  <div class="col-sm-offset-3 col-sm-9">
      <h2 *ngIf="!story">Program Indicators <span *ngIf="fiscalYear"> for FY{{fiscalYear.name}}</span></h2>
      <h2 *ngIf="story">Edit Program Indicators</h2><br>
  </div>
  <form class="form-horizontal form-label-left" novalidate (ngSubmit)="onSubmit()" [formGroup]="indicatorsForm">
      
      
      <div class="form-group">
          <label class="control-label col-md-3 col-sm-3 col-xs-12" for="majorProgramId">Major Program: </label>
          <div class="col-md-9 col-sm-9 col-xs-12">
              <select name="majorProgramId" id="majorProgramId" formControlName="majorProgramId" (change)="programChanged($event)" class="form-control col-md-7 col-xs-12" >
                  <option value="">--- select ---</option>
                  <optgroup  *ngFor="let initiative of initiatives" label="{{initiative.name}}">
                      <option *ngFor="let program of initiative.majorPrograms" [value]="program.id">{{program.name}} ({{program.pacCode}})</option>
                  </optgroup>
              </select>
          </div>
      </div>


      <div class="form-group" *ngIf="indicators != null && indicators.length > 0">
          <label class="control-label col-md-3 col-sm-3 col-xs-12" for="indicatorId">Program Indicator: </label>
          <div class="col-md-9 col-sm-9 col-xs-12">
              <select name="indicatorId" id="indicatorId" formControlName="indicatorId" (change)="indicatorChanged($event)" class="form-control col-md-7 col-xs-12" >
                  <option value="">--- select ---</option>
                  
                      <option *ngFor="let indicator of indicators" [value]="indicator.id" [innerHtml]="indicator.question"></option>

              </select>
          </div>
      </div>
      <div class="form-group"  *ngIf="indicatorsForm.value.indicatorId != ''">
          <label for="title" class="control-label col-md-3 col-sm-3 col-xs-12">Value:</label>        
          <div class="col-md-9 col-sm-9 col-xs-12">
              <input type="text" class="form-control col-xs-12" formControlName="value" style="width: 25%;" /><br><br><br>
          </div>
      </div>

      <div class="form-group" *ngIf="indicatorsForm.value.indicatorId != ''">
          <label for="title" class="control-label col-md-3 col-sm-3 col-xs-12">Audience Type:<br><small>Select all that apply</small></label>        
          <div class="col-md-9 col-sm-9 col-xs-12">
              <div class="checkbox">
                      <label class="">
                      <input type="checkbox" class="flat"><span>General community</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>Young children (Pre-K and younger)</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>K-12 students</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>Parents/families</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>Older adults/seniors</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>Agricultural workers/farmers</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>Professionals (e.g., educators, teachers, and healthcare providers)</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>Job seekers</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>Entrepreneurs</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>Businesses</span>
                      </label><br>
                      <label class="">
                      <input type="checkbox" class="flat"><span>Other (please specify):</span>
                      </label><br>
                      <input type="text" class="form-control col-xs-12" style="width: 50%;" /><br><br><br>
              </div>
          </div>
      </div>
      <div class="form-group"  *ngIf="indicatorsForm.value.indicatorId != ''">
          <label for="title" class="control-label col-md-3 col-sm-3 col-xs-12">Reach:<br><small>(i.e., number of program participants or attendees)</small></label>        
          <div class="col-md-9 col-sm-9 col-xs-12"><br>
              <input type="text" class="form-control col-xs-12" formControlName="reach"  style="width: 25%;" /><br><br><br><br><br><br>
          </div>
      </div>
      <div class="ln_solid"></div>
      <div class="form-group">
          <div class="col-md-6 col-sm-6 col-xs-12 col-md-offset-3">
              <a class="btn btn-primary" (click)="onCancel()">Cancel</a>
              <button type="submit" [disabled]="indicatorsForm.invalid"  class="btn btn-success">Submit</button>
          </div>
      </div>
      
  </form>
</div>


















  `,
  styles: [
  ]
})
export class IndicatorsDemoComponent implements OnInit {


  fiscalYear:FiscalYear;
  errorMessage:any;
  initiatives:StrategicInitiative[];
  indicators:Indicator[];
  indicatorsForm = null;
  loading = false;

  constructor( 
    private fb: FormBuilder,
    private programsService:ProgramsService,
    private indicatorsService: IndicatorsService,
    private fiscalYearService: FiscalyearService
)   
{
  this.indicatorsForm = this.fb.group(
    {
        majorProgramId: ["", Validators.required],
        indicatorId: ["", Validators.required],
        value: ["", Validators.required],
        audience: [""],
        reach: [""]
    }
);

}


  ngOnInit(): void {
    this.getFiscalYear();
  }


  getFiscalYear(){

        if( this.fiscalYear == null ){
            this.fiscalYearService.current("serviceLog", true).subscribe(
                res => {
                    this.fiscalYear =<FiscalYear> res;
                    this.getInitiatives();
                    
                },
                error => this.errorMessage = <any>error
            )
        }
        
    
}

getInitiatives(){
    this.programsService.listInitiatives(this.fiscalYear.name).subscribe(
        i => this.initiatives = i,
        error =>  this.errorMessage = <any>error
    );
}

programChanged(event){
  var mpId = this.indicatorsForm.value.majorProgramId;
  this.indicatorsService.indicatorsforprogram(mpId).subscribe(
      res => {
          this.indicators = res;
      }
  )
}

indicatorChanged(event){

}


onSubmit(){

}



}
