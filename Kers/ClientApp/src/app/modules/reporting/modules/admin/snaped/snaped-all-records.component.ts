import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'snaped-all-records',
    template: `
    <p>
    <activity-filter></activity-filter>
    </p>
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SnapedAllRecordsComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
