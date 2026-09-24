import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'activity-stats-filter',
    template: `
    <br><br>
    <activity-filter [userId]="0"></activity-filter>
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ActivityStatsFilterComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
