import { Component, OnInit, Input } from '@angular/core';
import { FormControl, FormBuilder } from '@angular/forms';
import { SnapedService } from '../../servicelog/snaped.service';
import { SnapEdActivityType, SnapEdProjectType, SnapEdCommitmentService, CommitmentBundle, SnapEdReinforcementItem } from '../snap-ed-commitment.service';

@Component({
  selector: 'commitment-form',
  templateUrl: './commitment-form.component.html',
  styleUrls: ['./commitment-form.component.css']
})
export class CommitmentFormComponent implements OnInit {

  @Input() commitment:CommitmentBundle;

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
    this.service.comitmentActivityTypes().subscribe(
      res => {
        this.activityTypes = <SnapEdActivityType[]>res;
        this.service.comitmentProjectTypes().subscribe(
          res => {
            this.projectTypes = <SnapEdProjectType[]>res;
            this.service.comitmentReinforcementItems().subscribe(
              res => {
                this.reinforcementItems = <SnapEdReinforcementItem[]>res;
                this.generateForm();
                this.loading = false;
              }
            );
            
          },
          err =>this.errorMessage = <any> err
        )
      },
      err =>this.errorMessage = <any> err
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
                  amount: [0, this.isPositiveInt],
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
                  amount: [0, this.isPositiveInt],
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
            amount: [0, this.isPositiveInt],
            snapEd_ActivityTypeId: option.id
        }));
      }
    }
    let reinforcementItemsArray = [];
    for(let item of this.reinforcementItems){
      reinforcementItemsArray.push(this.fb.group({
        selected: false,
        snapEd_ReinforcementItemId: item.id
      }))
    }

    //console.log(this.projectTypes);
    //console.log(this.activityTypes);
    //console.log(notPerProjectArray);


    this.commitmentForm = this.fb.group(
      {
      
          
        hoursCommitment: this.fb.array(hoursActivitiesArray),
        contactsCommitment: this.fb.array(contactsActivitiesArray),
        adminCommitment: this.fb.array(notPerProjectArray),
        items: this.fb.array(reinforcementItemsArray),
        suggestion: ""
          
      }
    ); 



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

  onSubmit(){
    console.log( this.commitmentForm.value );
  }
  onCancel(){

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
