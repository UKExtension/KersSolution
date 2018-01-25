import { Component, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css', './custom.scss', './custom-print.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class AppComponent {
  title = 'app';
}
