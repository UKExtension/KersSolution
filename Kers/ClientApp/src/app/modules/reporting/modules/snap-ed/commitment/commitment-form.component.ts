import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FormControl, FormBuilder } from '@angular/forms';
import { SnapedService } from '../../servicelog/snaped.service';
import { SnapEdActivityType, SnapEdProjectType, SnapEdCommitmentService, CommitmentBundle, SnapEdReinforcementItem, SnapEdReinforcementItemChoice } from '../snap-ed-commitment.service';
import { User } from '../../user/user.service';

@Component({
  selector: 'commitment-form',
  templateUrl: './commitment-form.component.html',
  styleUrls: ['./commitment-form.component.css']
})
export class CommitmentFormComponent implements OnInit {

  @Input() commitment:CommitmentBundle | null = null;
  @Input() commitmentFiscalYearId:number = 0;
  @Input() commitmentUserId:number = 0;

  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<CommitmentBundle>();

  commitmentForm = null;
  activityTypes: SnapEdActivityType[];
  projectTypes: SnapEdProjectType[];
  reinforcementItems: SnapEdReinforcementItem[];

  hoursActivityTypes: SnapEdActivityType[];
  contactsActivityTypes: SnapEdActivityType[];
  notPerProjectActivityTypes: SnapEdActivityType[];

  errorMessage:string;
  loading = false;
  hoursCounter = 0;
  contactsCounter = 0;

  constructor(
    private fb: FormBuilder,
    private service:SnapedService,
    private commitmentService:SnapEdCommitmentService
  ) { }

  ngOnInit() {
    this.loading = true;
    this.getActivityTypes();
  }
  getActivityTypes(){
    this.service.comitmentActivityTypes(this.commitmentFiscalYearId).subscribe(
      res => {
        this.activityTypes = <SnapEdActivityType[]>res;
        this.getProjectTypes();
      },
      err =>this.errorMessage = <any> err
    );
  }
  getProjectTypes(){
    this.service.comitmentProjectTypes(this.commitmentFiscalYearId).subscribe(
      res => {
        this.projectTypes = <SnapEdProjectType[]>res;  
        this.getReinforcementItems()      
      },
      err =>this.errorMessage = <any> err
    )
  }
  getReinforcementItems(){
    this.service.comitmentReinforcementItems(this.commitmentFiscalYearId).subscribe(
      res => {
        this.reinforcementItems = <SnapEdReinforcementItem[]>res;
        this.generateForm();
        this.loading = false;
      }
    );
  }

  generateForm(){
    let hoursActivitiesArray = [];
    for(let pt of this.projectTypes){
        this.hoursActivityTypes = [];
        for(let at of this.activityTypes){
          if(at.measurement == "Hour" && at.perProject){
            this.hoursActivityTypes.push(at);
            hoursActivitiesArray.push(this.fb.group({
                  amount: [this.getAmount(at, pt), this.isPositiveInt],
                  snapEd_ProjectTypeId: pt.id,
                  snapEd_ActivityTypeId: at.id
              }));                
          }
        }
    }

    let contactsActivitiesArray = [];
    for(let pt of this.projectTypes){
        this.contactsActivityTypes = [];
        for(let at of this.activityTypes){
          if(at.measurement == "Contact" && at.perProject){
            this.contactsActivityTypes.push(at);
            contactsActivitiesArray.push(this.fb.group({
                  amount: [this.getAmount(at, pt), this.isPositiveInt],
                  snapEd_ProjectTypeId: pt.id,
                  snapEd_ActivityTypeId: at.id
              }));                
          }
        }
    }

    let notPerProjectArray = [];
    this.notPerProjectActivityTypes = [];
    for( let option of this.activityTypes){
      if(!option.perProject){
        this.notPerProjectActivityTypes.push(option);
        notPerProjectArray.push(this.fb.group({
            amount: [this.getAmount(option), this.isPositiveInt],
            snapEd_ActivityTypeId: option.id
        }));
      }
    }
    let reinforcementItemsArray = [];
    for(let item of this.reinforcementItems){
      reinforcementItemsArray.push(this.fb.group({
        selected: this.getItemSelection(item),
        snapEd_ReinforcementItemId: item.id
      }))
    }
    this.commitmentForm = this.fb.group(
      {
        hoursCommitment: this.fb.array(hoursActivitiesArray),
        contactsCommitment: this.fb.array(contactsActivitiesArray),
        adminCommitment: this.fb.array(notPerProjectArray),
        items: this.fb.array(reinforcementItemsArray),
        suggestion: ""
      }
    ); 
    if(this.commitment != null){
      if( typeof this.commitment.suggestion === 'string'){
        this.commitmentForm.patchValue({suggestion:this.commitment.suggestion});
      }else{
        if(this.commitment.suggestion != null){
          this.commitmentForm.patchValue({suggestion:this.commitment.suggestion.suggestion});
        }
      }
    }
  }

  getItemSelection(item:SnapEdReinforcementItem):boolean{
    if(this.commitment != null){
      var val = [];
      if(item != null){
        val = this.commitment.items.filter( i => i.snapEd_ReinforcementItemId == item.id);
        if(val.length != 0){
          return true;
        }
      }
    }
    return false;
  }
  getAmount(type:SnapEdActivityType, project:SnapEdProjectType = null):number{
    if(this.commitment != null){
      var val = [];
      if(project == null){
        val = this.commitment.commitments.filter( c => c.snapEd_ActivityTypeId == type.id);
      }else{
        val = this.commitment.commitments.filter( c => c.snapEd_ActivityTypeId == type.id && c.snapEd_ProjectTypeId == project.id);
      }
      if(val.length != 0){
        return val[0].amount;
      }
    }
    return 0;
  }

  rIndex(){
    if(this.hoursCounter == this.commitmentForm.get("hoursCommitment").length){
        this.hoursCounter = 0
    }
    var val = ''+this.hoursCounter++
    return val;
  }
  cIndex(){
    if(this.contactsCounter == this.commitmentForm.get("contactsCommitment").length){
        this.contactsCounter = 0
    }
    var val = ''+this.contactsCounter++
    return val;
  }

  totalByActivityTypeHours(type:SnapEdActivityType):number{
    var sum = 0;
    for( let contr of this.commitmentForm.controls.hoursCommitment.controls){
        if(contr.value.snapEd_ActivityTypeId == type.id){
            sum += contr.value.amount;
        }
    }   
    return sum;
  }
  totalHours(){
    var sum = 0;
    for( let contr of this.commitmentForm.controls.hoursCommitment.controls){
      sum += contr.value.amount;
    }
    for( let contr of this.commitmentForm.controls.adminCommitment.controls){
      sum += contr.value.amount;
    }   
    return sum;
  }
  totalByActivityTypeContacts(type:SnapEdActivityType):number{
    var sum = 0;
    for( let contr of this.commitmentForm.controls.contactsCommitment.controls){
        if(contr.value.snapEd_ActivityTypeId == type.id){
            sum += contr.value.amount;
        }
    }   
    return sum;
  }

  onSubmit(){

    this.loading = true;
    var commitmens = this.commitmentForm.value.hoursCommitment.concat(this.commitmentForm.value.contactsCommitment, this.commitmentForm.value.adminCommitment);
    var commitmentBundle = <CommitmentBundle>{};
    commitmentBundle.commitments = commitmens;
    var items = [];
    for( var item of this.commitmentForm.value.items){
      if(item.selected){
        var itm = <SnapEdReinforcementItemChoice>{};
        itm.snapEd_ReinforcementItemId = item.snapEd_ReinforcementItemId;
        items.push(itm);
      }
    }
    commitmentBundle.items = items;
    commitmentBundle.suggestion = this.commitmentForm.value.suggestion;
    commitmentBundle.fiscalyearid = this.commitmentFiscalYearId;
    //console.log(commitmentBundle);
 
    this.commitmentService.addOrEditCommitment(commitmentBundle).subscribe(
      res => {
        this.onFormSubmit.emit(<CommitmentBundle> res);
        this.loading = false;
      }
    );

  }
  onCancel(){
    this.onFormCancel.emit();
  }

  /************************
      
      Validators
    
     ***********************/

    isIntOrFloat(control:FormControl){
      if(control.value == +control.value && +control.value >= 0){
          return null;
      }
      return {"notDigit":true};
  }

  isPositiveInt(control:FormControl){
      
      if(!isNaN(control.value) && (function(x) { return (x | 0) === x; })(parseFloat(control.value)) && +control.value >= 0){
          return null;
      }
      return {"notInt":true};
  }

}
