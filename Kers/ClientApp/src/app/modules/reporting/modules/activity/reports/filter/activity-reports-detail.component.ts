import { Component, OnInit, Input } from '@angular/core';
import { ActivitySearchResult } from '../../activity.service';

@Component({
  selector: '[activity-reports-detail]',
  templateUrl: './activity-reports-detail.component.html'
})
export class ActivityReportsDetailComponent implements OnInit {
  @Input('activity-reports-detail') revision:ActivitySearchResult;
  details = false;
  constructor() { }

  ngOnInit() {
  }

}
