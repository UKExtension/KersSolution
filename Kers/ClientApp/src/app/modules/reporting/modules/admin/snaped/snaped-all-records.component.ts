import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'snaped-all-records',
  template: `
    <p>
    <activity-filter></activity-filter>
    </p>
  `,
  styles: [
  ]
})
export class SnapedAllRecordsComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
