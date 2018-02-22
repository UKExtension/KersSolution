import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-commitment-home',
  templateUrl: './commitment-home.component.html',
  styles: []
})
export class CommitmentHomeComponent implements OnInit {
  commitment;
  constructor(
    private route: ActivatedRoute,
  ) { }

  ngOnInit() {
    this.getCommitment();
  }
  getCommitment(): void {
    const userid = this.route.snapshot.paramMap.get('userid');

    console.log(userid);
    const fiscalyearid = this.route.snapshot.paramMap.get('fiscalyearid');

    console.log(fiscalyearid);

    /* this.heroService.getHero(id)
      .subscribe(hero => this.hero = hero); */
  }

}
