import { Component, OnInit, Input } from '@angular/core';
import { SnapSearchResult } from './snaped-admin.service';

@Component({
  selector: '[snaped-reports-detail]',
  templateUrl: './snaped-reports-detail.component.html'
})
export class SnapedReportsDetailComponent implements OnInit {
  @Input('snaped-reports-detail') revision:SnapSearchResult;
  details = false;
  constructor() { }

  ngOnInit() {
  }

}
