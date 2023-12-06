import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { PlanningUnit, User, UserService } from '../user/user.service';
import { FormArray, FormBuilder, FormControl, Validators } from '@angular/forms';
import { ReportingService } from '../../components/reporting/reporting.service';
import { TaxExempt, TaxExemptFinancialYear, TaxExemptFundsHandled, TaxExemptProgramCategory } from './exmpt';
import { PlanningunitService } from '../planningunit/planningunit.service';
import { ExemptService } from './exempt.service';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { number } from 'echarts';

@Component({
  selector: 'exempt-form',
  templateUrl: './exempt-form.component.html',
  styles: [
  ]
})
export class ExemptFormComponent implements OnInit {
  

  countyEvent = false;

  @Input()exempt:TaxExempt;

  ExemptForm:any;
  planningUnits: PlanningUnit[];
  planningUnitsOptions = Array<any>();
  county:PlanningUnit;
  countyId:number;
  user:User;
  financialYears:Observable<TaxExemptFinancialYear[]>;
  fundsHandled:TaxExemptFundsHandled[];

  programCategories:TaxExemptProgramCategory[] = [];
  loading:boolean = true;
  is501:boolean = false;
  isBoard:boolean = false;


  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<TaxExempt>();

  constructor(
    private fb: FormBuilder,
    private planningUnitService: PlanningunitService,
    private userService:UserService,
    private service:ExemptService
  ) { }

  ngOnInit(): void {
    this.financialYears = this.service.financialYears();
     this.service.fundsHandled().subscribe(
      res => this.fundsHandled = res
     )
    
    this.service.programCategories().subscribe(
      res => {
        this.programCategories = res;
        


        let opArray = [];

        for( let option of this.programCategories){
            opArray.push(this.fb.group({
                selected: false,
                taxExemptProgramCategoryId: option.id
            }));
        };
        
        this.ExemptForm = this.fb.group(
          {
            name:["", Validators.required],
            ein:"",
            bankName:"",
            bankAccountName:'',
            areas: '',
            taxExemptFinancialYearId:'',
            taxExemptProgramCategories:this.fb.array(opArray),
            donorsReceivedAck:"",
            annBudget:"",
            annFinancialRpt:"",
            annAuditRpt:"",
            annInvRpt:"",
            handledId:["", Validators.required],
            districtName:"",
            districtEin:"",
            organizationName:"",
            organizationEin:"",
            organizationResidesId:"",
            organizationLetterDate:"",
            organizationSignedDate:"",
            organizationAppropriate:""
          }
        );

        this.ExemptForm.controls['handledId'].valueChanges.subscribe((change: number) => {
          var filtered = this.fundsHandled.filter( r => r.id == change);
          if(filtered.length == 1){
            if(filtered[0].is501 ){
              this.is501=true;
              this.isBoard = false;
            }else{
              this.is501=false;
              this.isBoard = true;
            }
          }
        });
      } 
    );

    this.userService.current().subscribe(
      res =>{
        this.county = res.rprtngProfile.planningUnit;
        this.countyId = res.rprtngProfile.planningUnitId;
        var cntId = res.rprtngProfile.planningUnitId;
        this.user = res;
        this.planningUnitService.counties().subscribe(
          res =>{
            this.planningUnits = res;
            var optns = Array<any>();
            this.planningUnits.forEach(function(element){
              if(element.id != cntId){
                optns.push(
                  { value: element.id, label: element.name}
                );
              }
              
            });
            if(!this.exempt){
              if(this.planningUnits.filter(res => res.id == this.countyId).length > 0){
                this.ExemptForm.patchValue({
                  areas:[{value:this.countyId, label:this.county.name}]
                });
              }
            }else{
              this.ExemptForm.patchValue(this.exempt);
              var cats = [];
              for( var ct of this.programCategories){
                if( this.exempt.taxExemptProgramCategories.filter( c => c.taxExemptProgramCategoryId == ct.id).length > 0){
                  cats.push({
                    selected: true,
                    taxExemptProgramCategoryId: ct.id
                  });
                }else{
                  cats.push({
                    selected: false,
                    taxExemptProgramCategoryId: ct.id
                  });
                }
              }
              this.ExemptForm.patchValue({
                areas:this.exempt.areas.map( a => ({value: a.unitId, label:a.unit.name})),
                taxExemptProgramCategories:cats
              });
            }

            this.planningUnitsOptions = optns;
            this.loading = false;
          });
      });
      
  }


  onSubmit(){

    this.loading = true;
    var exmpt = <TaxExempt> this.ExemptForm.value;
    var countiesSelection = this.ExemptForm.value.areas;
    exmpt.areas = countiesSelection.map( u => ({unitId: u.value}));
    var categoriesSelection = this.ExemptForm.value.taxExemptProgramCategories;
    exmpt.taxExemptProgramCategories = categoriesSelection.filter(c => c.selected == true).map( c => ({taxExemptProgramCategoryId:c.taxExemptProgramCategoryId}))
    console.log(exmpt);
    if(this.exempt){
      this.service.update(this.exempt.id, exmpt).subscribe(
        res => {
          this.loading = false;
          this.onFormSubmit.emit(res);
        }
      );
    }else{
      this.service.addExempt(exmpt).subscribe(
        res => {
          this.loading = false;
          this.onFormSubmit.emit(res);
        }
      );

    }
    

  }

  onCancel(){
    this.onFormCancel.emit();
  }


}