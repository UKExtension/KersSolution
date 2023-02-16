import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'custom-reports',
  template: `
  <a [routerLink]="['/reporting/admin/snaped/customreports']" routerLinkActive="active" [routerLinkActiveOptions]="{exact: true}">Snap-Ed Records</a> | <a [routerLink]="['/reporting/admin/snaped/customreports/all']" routerLinkActive="active">All Records</a> | <a [routerLink]="['/reporting/admin/snaped/customreports/time']" routerLinkActive="active">Time Spent Teaching</a>
    <router-outlet></router-outlet>
  `,
  styles: [
    `
    .active{
      font-weight:bold;
    }
    `
  ]
})
export class CustomReportsComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
