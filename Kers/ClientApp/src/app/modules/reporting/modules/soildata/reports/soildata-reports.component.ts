import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-soildata-reports',
    template: `
    <p>
      soildata-reports works!
    </p>
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SoildataReportsComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
