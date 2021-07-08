import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SnapEdCommitmentService, CommitmentBundle } from '../snap-ed-commitment.service';
import { ReportingService } from '../../../components/reporting/reporting.service';

@Component({
  selector: 'app-commitment-home',
  templateUrl: './commitment-home.component.html',
  styles: []
})
export class CommitmentHomeComponent implements OnInit {
  commitment:CommitmentBundle;
  @Input() userid = 0;
  @Input() fiscalyearid = 0;
  @Input() canItBeEdited = true;
 
  isItJustView = true;

  constructor(
    private route: ActivatedRoute,
    private service: SnapEdCommitmentService,
    private reportingService: ReportingService,
  ) { }

  ngOnInit() {
    this.reportingService.setTitle('Commitment Worksheet');
    this.getCommitment();
  }
  getCommitment(): void {
    const routeUserId = this.route.snapshot.paramMap.get('userid');
    if(routeUserId){
      this.userid = +routeUserId;
    }
    const routeFiscalYear = this.route.snapshot.paramMap.get('fiscalyearid');
    if(routeFiscalYear){
      this.fiscalyearid = +routeFiscalYear;
    }
    
    this.service.getSnapCommitments(this.userid, this.fiscalyearid).subscribe(
      res => {
            this.commitment = <CommitmentBundle> res;
            if(this.commitment.commitments.length == 0){
              this.isItJustView = false;
            }
            //console.log( this.commitment );
        }
    );
  }
  commitmentEdited(event:CommitmentBundle){
    this.commitment = event;
    this.isItJustView = true;
  }

}
