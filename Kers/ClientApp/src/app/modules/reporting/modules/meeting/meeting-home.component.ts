import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'meeting-home',
  template: `
    <router-outlet></router-outlet>
  `,
  styles: []
})
export class MeetingHomeComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
