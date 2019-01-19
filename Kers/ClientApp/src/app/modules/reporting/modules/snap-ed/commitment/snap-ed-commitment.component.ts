import { Component, OnInit, Input } from '@angular/core';
import { SnapEdCommitmentService, CommitmentBundle } from '../snap-ed-commitment.service';
import { FiscalYear } from '../../admin/fiscalyear/fiscalyear.service';

@Component({
  selector: 'snap-ed-commitment-manager',
  template: `
    <fiscal-year-switcher [type]="'snapEd'" *ngIf="displayFiscalYearSwitcher" (onSwitched)="fiscalYearSwitched($event)"></fiscal-year-switcher>
    <loading *ngIf="loading"></loading>
    <div *ngIf="!loading">
      <div *ngIf="isItJustView" >
          <commitment-view [commitment]="commitment"  [commitmentFiscalYear]="fiscalyear" ></commitment-view>
          <a class="btn btn-dark btn-lg btn-block" (click)="isItJustView=false" *ngIf="canItBeEdited">Edit Commitment Data</a>
      </div>
      
      <commitment-form *ngIf="!isItJustView && !confirmDelete" [commitment]="commitment" [commitmentFiscalYear]="fiscalyear" [commitmentUserId]="userid" (onFormSubmit)="commitmentEdited($event)" (onFormCancel)="isItJustView=true"></commitment-form>
      <div *ngIf="!isItJustView && commitment.commitments.length!=0 && !confirmDelete" class="text-right" (click)="confirmDelete=true"><button type="button" class="btn btn-danger btn-sm">delete</button></div>
      <div *ngIf="confirmDelete"><br><br>
      Do you really want to delete commitment for FY{{fiscalyear.name}}?<br><button (click)="confirmDeleteCommitment()" class="btn btn-info btn-xs">Yes</button> <button (click)="confirmDelete=false" class="btn btn-info btn-xs">No</button>
      </div>
  </div>
  `,
  styles: []
})
export class SnapEdCommitmentComponent implements OnInit {

  @Input() userid = 0;
  @Input() displayFiscalYearSwitcher = true;
  @Input() fiscalyear:FiscalYear;
  @Input() canItBeEdited = true;

  isItJustView = true;
  commitment:CommitmentBundle;
  loading = true;
  confirmDelete = false;

  constructor(
    private service: SnapEdCommitmentService
  ) { }

  ngOnInit() {
    if(!this.displayFiscalYearSwitcher){
      this.getCommitment();
    }
  }


  getCommitment(): void {
    this.loading = true;
    
    this.service.getSnapCommitments(this.userid, this.fiscalyear.id).subscribe(
      res => {
            this.commitment = <CommitmentBundle> res;
            this.loading = false;
        }
    );
  }

  confirmDeleteCommitment(){
    this.loading = true;
    this.service.deleteSnapCommitments(this.userid, this.fiscalyear.name)
      .subscribe(
        _ => {
          this.confirmDelete = false;
          this.loading = false;
        }
      )
    
  }

  commitmentEdited(event){
    this.commitment = event;
    this.isItJustView = true;
  }

  fiscalYearSwitched(event:FiscalYear){
    this.fiscalyear = event;
    this.getCommitment();
  }

}
