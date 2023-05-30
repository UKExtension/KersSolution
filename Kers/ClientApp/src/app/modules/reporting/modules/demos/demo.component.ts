import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'demo',
  template: `
    <p>
      demo works!
    </p>
  `,
  styles: [
  ]
})
export class DemoComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
