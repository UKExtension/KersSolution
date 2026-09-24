import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';
import { LadderApplication } from './ladder';

@Component({
    selector: '[ladder-filter-detail]',
    templateUrl: './ladder-filter-detail.component.html',
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class LadderFilterDetailComponent implements OnInit {

  @Input('ladder-filter-detail') application:LadderApplication;

  details = false;

  constructor() { }

  ngOnInit() {
  }

}
