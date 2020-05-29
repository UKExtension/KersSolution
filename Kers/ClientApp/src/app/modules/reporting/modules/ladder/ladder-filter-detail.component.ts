import { Component, OnInit, Input } from '@angular/core';
import { LadderApplication } from './ladder';

@Component({
  selector: '[ladder-filter-detail]',
  templateUrl: './ladder-filter-detail.component.html',
  styles: []
})
export class LadderFilterDetailComponent implements OnInit {

  @Input('ladder-filter-detail') application:LadderApplication;

  details = false;

  constructor() { }

  ngOnInit() {
  }

}
