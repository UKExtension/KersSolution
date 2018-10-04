import { Component, Input, Output, EventEmitter } from '@angular/core';
import { PlanningUnit, User } from '../../user/user.service';
import { SnapedService } from '../snaped.service';
import { Observable } from 'rxjs/Observable';
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';


@Component({
  selector: 'snape-ed-commitment-stats',
  templateUrl: 'snaped-commitment-stats.component.html'
})
export class SnapedCommitmentStatsComponent { 
    @Input() planningUnit: PlanningUnit;
    @Input() user:User;
    @Input() fiscalYear:FiscalYear;

    @Output() onCalculated = new EventEmitter<number>();


    projectTypes;
    commitments;

    errorMessage:string;

    constructor( 
      private service:SnapedService,
    )   
    {}

    ngOnInit(){
      
      
        this.service.comitmentProjectTypes().subscribe(
          res => {
            this.projectTypes = res;
            if(this.user != null){
              this.service.commitmentPerIndividual(this.user.id, this.fiscalYear.name).subscribe(
                res => {
                  this.commitments = res;
                },
                err => this.errorMessage = <any>err
              )
            }else if(this.planningUnit != null){
              this.service.commitmentPerCounty(this.planningUnit.id).subscribe(
                res => {
                  this.commitments = res;
                },
                err => this.errorMessage = <any>err
              )
            }
          },
          err => this.errorMessage = <any> err
        )
     
    }
    hours(projectId){
      var totalHours = 0;
      var filtered = this.commitments.filter( c => c.snapEd_ProjectTypeId == projectId );
      for(let cmtmnt of filtered){
        totalHours += cmtmnt.amount;
      }

      return totalHours;
    }

    admingHours(){
      var totalHours = 0;
      var filtered = this.commitments.filter( c => c.snapEd_ProjectTypeId == null );
      for(let cmtmnt of filtered){
        totalHours += cmtmnt.amount;
      }

      return totalHours;
    }

    totalHours(){
      var totalHours = 0;
      for(let cmtmnt of this.commitments){
        totalHours += cmtmnt.amount;
      }
      this.onCalculated.emit(totalHours);
      return totalHours;
    }

    

}