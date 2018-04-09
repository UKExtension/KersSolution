import { Component, OnInit, Input } from '@angular/core';
import { SnapEdCommitmentService, CommitmentBundle } from '../snap-ed-commitment.service';
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';

@Component({
  selector: 'snap-ed-commitment-manager',
  template: `
    <fiscal-year-switcher [type]="'snapEd'" *ngIf="displayFiscalYearSwitcher" (onSwitched)="fiscalYearSwitched($event)"></fiscal-year-switcher>
    <div *ngIf="commitment!=null">
      <div *ngIf="isItJustView" >
          <commitment-view [commitment]="commitment"  [commitmentFiscalYearId]="fiscalyearid" ></commitment-view>
          <a class="btn btn-dark btn-lg btn-block" (click)="isItJustView=false" *ngIf="canItBeEdited">Edit Commitment Data</a>
      </div>
      
      <commitment-form *ngIf="!isItJustView" [commitment]="commitment" [commitmentFiscalYearId]="fiscalyearid" (onFormSubmit)="commitmentEdited($event)" (onFormCancel)="isItJustView=true"></commitment-form>
      
  </div>
  `,
  styles: []
})
export class SnapEdCommitmentComponent implements OnInit {

  @Input() userid = 0;
  @Input() displayFiscalYearSwitcher = true;
  @Input() fiscalyearid = 0;
  @Input() canItBeEdited = true;

  isItJustView = true;
  commitment:CommitmentBundle;

  constructor(
    private service: SnapEdCommitmentService
  ) { }

  ngOnInit() {
    if(!this.displayFiscalYearSwitcher){
      this.getCommitment();
    }
  }


  getCommitment(): void {
    
    
    this.service.getSnapCommitments(this.userid, this.fiscalyearid).subscribe(
      res => {
            this.commitment = <CommitmentBundle> res;
            
            /* 
            
            if(this.commitment.commitments.length == 0){
              this.isItJustView = false;
            }

             */
            //console.log( this.commitment );
        }
    );
  }

  commitmentEdited(event){
    this.commitment = event;
    this.isItJustView = true;
  }

  fiscalYearSwitched(event:FiscalYear){
    this.fiscalyearid = event.id;
    this.getCommitment();
  }

}
