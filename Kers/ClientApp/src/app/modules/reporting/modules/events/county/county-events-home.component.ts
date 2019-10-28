import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-county-events-home',
  template: `
  <county-event-form></county-event-form>
    <p>
      county-events-home works!
    </p>
    <location-browser></location-browser>
  `,
  styles: []
})
export class CountyEventsHomeComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
