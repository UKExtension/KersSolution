import { Component, OnInit, Input } from '@angular/core';
import { CommitmentBundle, SnapEdCommitmentService, SnapEdActivityType, SnapEdProjectType, SnapEdReinforcementItem } from '../snap-ed-commitment.service';
import { SnapedService } from '../../servicelog/snaped.service';
import { FiscalYear, FiscalyearService } from '../../admin/fiscalyear/fiscalyear.service';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'commitment-view',
  templateUrl: './commitment-view.component.html',
  styleUrls: ['./commitment-view.component.css']
})
export class CommitmentViewComponent implements OnInit {
  @Input() commitment:CommitmentBundle | null = null;
  @Input() commitmentFiscalYearId:number = 0;
  @Input() commitmentUserId:number = 0;


  fiscalYear:Observable<FiscalYear>;
  loading = false;
  activityTypes: SnapEdActivityType[];
  projectTypes: SnapEdProjectType[];
  errorMessage:string;
  reinforcementItems: SnapEdReinforcementItem[];


  hoursActivityTypes: SnapEdActivityType[];
  contactsActivityTypes: SnapEdActivityType[];
  notPerProjectActivityTypes: SnapEdActivityType[];

  constructor(
    private service:SnapedService,
    private commitmentService:SnapEdCommitmentService,
    private fiscalYearService:FiscalyearService
  ) { }

  ngOnInit() {
    this.loading = true;
    this.getActivityTypes();
    this.fiscalYear = this.fiscalYearService.byId(this.commitmentFiscalYearId);
    
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
        this.divideCommitments()
        this.loading = false;
      }
    );
  }
  divideCommitments(){
    for(let pt of this.projectTypes){
        this.hoursActivityTypes = [];
        for(let at of this.activityTypes){
          if(at.measurement == "Hour" && at.perProject){
            this.hoursActivityTypes.push(at);               
          }
        }
    }

    for(let pt of this.projectTypes){
        this.contactsActivityTypes = [];
        for(let at of this.activityTypes){
          if(at.measurement == "Contact" && at.perProject){
            this.contactsActivityTypes.push(at);              
          }
        }
    }

    this.notPerProjectActivityTypes = [];
    for( let option of this.activityTypes){
      if(!option.perProject){
        this.notPerProjectActivityTypes.push(option);
      }
    }
  }


  valuePerCommitment(type:SnapEdActivityType, project:SnapEdProjectType | null = null){
    var commitment = []
    if(project != null){
      commitment = this.commitment.commitments.filter( a => a.snapEd_ActivityTypeId == type.id && a.snapEd_ProjectTypeId == project.id);
    }else{
      commitment = this.commitment.commitments.filter( a => a.snapEd_ActivityTypeId == type.id);
    }
    if(commitment.length > 0){
      return commitment[0].amount;
    }
    return 0;
  }
  totalPerType(type:SnapEdActivityType):number{
    var sum = 0;
    var commitments = this.commitment.commitments.filter( a => a.snapEd_ActivityTypeId == type.id);
    for( let com of commitments){
      sum += com.amount;
    }
    return sum;
  }
  totalHours(){
    var sum = 0;

    var hourTypes = this.activityTypes.filter(t=>t.measurement == "Hour");
    var typeIds = [];
    for( let tp of hourTypes){
      typeIds.push(tp.id);
    }

    var commitments = this.commitment.commitments.filter( a => typeIds.includes(a.snapEd_ActivityTypeId));
    for( let com of commitments){
      sum += com.amount;
    }
    return sum;
  }
  isItemSelected( item ):boolean{
    if( this.commitment.items.filter(i => i.snapEd_ReinforcementItemId == item.id).length > 0){
      return true;
    }
    return false;
  }
  getSuggestion(){
    
    if(typeof this.commitment.suggestion === 'string'){
      return this.commitment.suggestion;
    }else if(this.commitment.suggestion != null){
      return this.commitment.suggestion.suggestion;
    }
    return "";
  }

}
