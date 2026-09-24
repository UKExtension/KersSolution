import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
    template: `
    <h2>Reporting List</h2>
    <router-outlet></router-outlet>
  `,
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class ReportingListComponent { }