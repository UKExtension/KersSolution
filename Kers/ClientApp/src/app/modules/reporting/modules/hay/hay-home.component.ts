import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'hay-home',
  template: `
    <p>
      hay-home works!
    </p>
    <hay-sample-form></hay-sample-form>
  `,
  styles: [
  ]
})
export class HayHomeComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
