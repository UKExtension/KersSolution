import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'activity-stats-filter',
  template: `
    <br><br>
    <activity-filter [userId]="0"></activity-filter>
  `,
  styles: [
  ]
})
export class ActivityStatsFilterComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
