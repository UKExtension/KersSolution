import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'alert-widget',
  template: `
  <div class="alert alert-success alert-dismissible " role="alert">
  <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span>
  </button>
  <strong>Holy guacamole!</strong> Best check yo self, you're not looking too good.
  </div>
  `,
  styles: [
  ]
})
export class AlertWidgetComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
