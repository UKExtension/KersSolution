import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';

@Component({
    selector: 'app-snap-ed-home',
    templateUrl: './snap-ed-home.component.html',
    styleUrls: ['./snap-ed-home.component.css'],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class SnapEdHomeComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
