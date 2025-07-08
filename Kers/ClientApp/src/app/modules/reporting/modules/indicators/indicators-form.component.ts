import { Component, Input, OnInit } from '@angular/core';
import { Indicator, IndicatorsService } from './indicators.service';
import { MajorProgram, ProgramsService, StrategicInitiative } from '../admin/programs/programs.service';
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';
import { ReportingService } from '../../components/reporting/reporting.service';
import { FormBuilder } from '@angular/forms';

@Component({
  selector: 'indicators-form',
  templateUrl: './indicators-form.component.html',
  styles: [
  ]
})
export class IndicatorsFormComponent implements OnInit {

  @Input() fiscalYear:FiscalYear;
  @Input() demoMode:boolean = false;
  initiatives:StrategicInitiative[];
  programs: MajorProgram[];
  selectedProgram: MajorProgram;
  selectedIndicators: Indicator[];
  childIcon:string = '(Youth)';
  adultIcon:string = '(Adult)';
  volunteersIcon:string = '(Volunteers)';

  loading = false;

  errorMessage:string;
  indicatorsForm = null;

  dataSubmitted = false;
  
//<fiscal-year-switcher [initially]="current" (onSwitched)="fiscalYearSwitched($event)"></fiscal-year-switcher>
  constructor( 
      private fb: FormBuilder,
      private programsService:ProgramsService,
      private indicatorsService:IndicatorsService
  )   
  {
      this.indicatorsForm = this.fb.group({
          indicatorsGroup: fb.array([])
      });

  }

  ngOnInit(){
      
      var prgrms = [];
      this.initiatives = null;
      this.selectedProgram = null;
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

  

}
