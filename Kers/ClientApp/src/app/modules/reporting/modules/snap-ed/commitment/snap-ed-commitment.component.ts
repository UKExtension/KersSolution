import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';
import { SnapEdCommitmentService, CommitmentBundle } from '../snap-ed-commitment.service';
import { FiscalYear, FiscalyearService } from '../../admin/fiscalyear/fiscalyear.service';

@Component({
    selector: 'snap-ed-commitment-manager',
    template: `
    @if (displayFiscalYearSwitcher) {
      <fiscal-year-switcher [type]="'snapEd'" (onSwitched)="fiscalYearSwitched($event)"></fiscal-year-switcher>
    }
    @if (loading) {
      <loading></loading>
    }
    @if (!loading) {
      <div>
        @if (isItJustView) {
          <div >
            <commitment-view [commitment]="commitment"  [commitmentFiscalYear]="fiscalyear" ></commitment-view>
            @if (canItBeEdited) {
              <a class="btn btn-dark btn-lg btn-block" (click)="isItJustView=false">Edit Commitment Data</a>
            }
          </div>
        }
        @if (!isItJustView && !confirmDelete) {
          <commitment-form [commitment]="commitment" [commitmentFiscalYear]="fiscalyear" [commitmentUserId]="userid" (onFormSubmit)="commitmentEdited($event)" (onFormCancel)="isItJustView=true"></commitment-form>
        }
        @if (!isItJustView && commitment.commitments.length!=0 && !confirmDelete && !loading && showDelete) {
          <div class="text-right" (click)="confirmDelete=true"><button type="button" class="btn btn-danger btn-sm">delete</button></div>
        }
        @if (confirmDelete) {
          <div><br><br>
            Do you really want to delete commitment for FY{{fiscalyear.name}}?<br><button (click)="confirmDeleteCommitment()" class="btn btn-info btn-xs">Yes</button> <button (click)="confirmDelete=false" class="btn btn-info btn-xs">No</button>
          </div>
        }
      </div>
    }
    `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SnapEdCommitmentComponent implements OnInit {

  @Input() userid = 0;
  @Input() displayFiscalYearSwitcher = true;
  @Input() fiscalyear:FiscalYear;
  @Input() canItBeEdited = true;
  @Input() showDelete = true;

  isItJustView = true;
  commitment:CommitmentBundle;
  loading = true;
  confirmDelete = false;

  constructor(
    private service: SnapEdCommitmentService,
    private fiscalYearService : FiscalyearService
  ) { }

  ngOnInit() {
    if(!this.displayFiscalYearSwitcher){
      this.fiscalYearService.next("snapEd").subscribe(
        res => {
          this.fiscalyear = res;
          this.getCommitment();
        }
      )
      
    }
  }


  getCommitment(): void {
    this.loading = true;
    
    this.service.getSnapCommitments(this.userid, this.fiscalyear.id).subscribe(
      res => {
            this.commitment = <CommitmentBundle> res;
            if( this.commitment.commitments.length == 0 && !this.displayFiscalYearSwitcher ) this.isItJustView = false;
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
          this.getCommitment();
          this.isItJustView = true;
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
