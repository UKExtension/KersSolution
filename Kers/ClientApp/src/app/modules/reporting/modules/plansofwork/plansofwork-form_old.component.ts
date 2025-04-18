import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { PlansofworkService, Map, PlanOfWork } from './plansofwork.service';
import { FormBuilder, Validators }   from '@angular/forms';
import {ProgramsService, StrategicInitiative, MajorProgram} from '../admin/programs/programs.service';
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';


@Component({
    selector: 'planofwork-form-old',
    templateUrl: 'plansofwork-form_old.component.html'
})
export class PlansofworkForm_OldComponent implements OnInit{

    planofworkForm = null;
    @Input() planofwork = null;
    @Input() fiscalYear:FiscalYear;
    errorMessage: string;

    loading = false;
    options: object;
    initiatives:StrategicInitiative[];
    programs: MajorProgram[];
    maps: Map[];

    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<void>();

    constructor( 
        private plansofworkService: PlansofworkService,
        private service:ProgramsService,
        private fb: FormBuilder
    ){
        this.programs = [];
        this.options = { 
            placeholderText: 'Your Description Here!',
            toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL'],
            toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
            toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
            quickInsertButtons: ['ul', 'ol', 'hr'],    
        }

        this.planofworkForm = fb.group(
            {
              map: ['', Validators.required],
              title: ['', Validators.required],
              agentsInvolved: [''],
              mp1: ['', Validators.required],
              mp2: [''],
              mp3: [''],
              mp4: [''],
              situation: '',
              longTermOutcomes: '',
              intermediateOutcomes: '',
              initialOutcomes: '',
              learning: 'Audience:<br />Project or Activity:<br />Content or Curriculum:<br />Inputs:<br />Date:<br /><br />Audience:<br />Project or Activity:<br />Content or Curriculum: <br />Inputs:<br />Date:<br /><br />Audience:<br />Project or Activity:<br />Content or Curriculum:<br />Inputs:<br />Date: ',
              evaluation: 'Initial Outcome:<br />Indicator:<br />Method:<br />Timeline:<br />Intermediate Outcome:<br />Indicator:<br />Method:<br />Timeline:<br /><br />Long-term Outcome:<br />Indicator:<br />Method:<br />Timeline:'
            }
        );

    }
   
    ngOnInit(){

        this.plansofworkService.listMaps(this.fiscalYear.name).subscribe(
            m => this.maps = m,
            error =>  this.errorMessage = <any>error
        );
        var prgrms = [];
        this.service.listInitiatives(this.fiscalYear.name).subscribe(
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
        if(this.planofwork){
           this.planofworkForm.patchValue(this.planofwork);
           this.planofworkForm.patchValue({map:this.planofwork.map.id})
           if(this.planofwork.mp1 != null){
               this.planofworkForm.patchValue({mp1:this.planofwork.mp1.id})
           }
           if(this.planofwork.mp2 != null){
               this.planofworkForm.patchValue({mp2:this.planofwork.mp2.id})
           }
           if(this.planofwork.mp3 != null){
               this.planofworkForm.patchValue({mp3:this.planofwork.mp3.id})
           }
           if(this.planofwork.mp4 != null){
               this.planofworkForm.patchValue({mp4:this.planofwork.mp4.id})
           }
        }
        
    }

    onSubmit(){ 

        this.loading = true;

        var i = <PlanOfWork> this.planofworkForm.value;
        
        i.map = this.maps.find(c=>c.id = this.planofworkForm.value.map);

        i.mp1 = this.programs.find(p=>p.id == this.planofworkForm.value.mp1);

        if(this.planofworkForm.value.mp2 != null){
            i.mp2 = this.programs.find(p=>p.id == this.planofworkForm.value.mp2);
        }
        if(this.planofworkForm.value.mp3 != null){
            i.mp3 = this.programs.find(p=>p.id == this.planofworkForm.value.mp3);
        }
        if(this.planofworkForm.value.mp4 != null){
            i.mp4 = this.programs.find(p=>p.id == this.planofworkForm.value.mp4);
        }
      

        if(this.planofwork){
            this.plansofworkService.updatePlan(this.planofwork.id, this.planofworkForm.value).
            subscribe(
                res => {
                    this.planofwork = <PlanOfWork> res;
                    this.onFormSubmit.emit();
                    this.loading = false;
                    //console.log(res);
                }
            );
        }else{
            var fy = this.fiscalYear == null ? "0" : this.fiscalYear.name;
            this.plansofworkService.addPlan(i, fy).
            subscribe(
                res => {
                    this.onFormSubmit.emit();
                    this.loading = false;
                    //console.log(res.json());
                }
            );
            
        }

    }

    OnCancel(){
        this.onFormCancel.emit();
    }   
}