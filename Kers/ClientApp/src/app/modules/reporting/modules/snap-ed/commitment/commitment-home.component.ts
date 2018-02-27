import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SnapEdCommitmentService, CommitmentBundle } from '../snap-ed-commitment.service';
import { Observable } from 'rxjs/Observable';

@Component({
  selector: 'app-commitment-home',
  templateUrl: './commitment-home.component.html',
  styles: []
})
export class CommitmentHomeComponent implements OnInit {
  commitment:CommitmentBundle;
  constructor(
    private route: ActivatedRoute,
    private service: SnapEdCommitmentService
  ) { }

  ngOnInit() {
    //this.getCommitment();
  }
  getCommitment(): void {
    const userid = this.route.snapshot.paramMap.get('userid');
    const fiscalyearid = this.route.snapshot.paramMap.get('fiscalyearid');
    this.service.getSnapCommitments().subscribe(
      res => {
            this.commitment = <CommitmentBundle> res;
            console.log( this.commitment );
        }
    );
  }

}
