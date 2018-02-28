import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SnapEdCommitmentService, CommitmentBundle } from '../snap-ed-commitment.service';
import { Observable } from 'rxjs/Observable';
import { ReportingService } from '../../../components/reporting/reporting.service';

@Component({
  selector: 'app-commitment-home',
  templateUrl: './commitment-home.component.html',
  styles: []
})
export class CommitmentHomeComponent implements OnInit {
  commitment:CommitmentBundle;


  isItJustView = true;

  constructor(
    private route: ActivatedRoute,
    private service: SnapEdCommitmentService,
    private reportingService: ReportingService,
  ) { }

  ngOnInit() {
    this.reportingService.setTitle('Commitment Worksheet for October 1, 2018 - September 30, 2019');
    this.getCommitment();
  }
  getCommitment(): void {
    const userid = this.route.snapshot.paramMap.get('userid');
    const fiscalyearid = this.route.snapshot.paramMap.get('fiscalyearid');
    this.service.getSnapCommitments().subscribe(
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
