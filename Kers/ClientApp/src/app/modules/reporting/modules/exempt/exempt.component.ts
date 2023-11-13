import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'exempt',
  templateUrl: './exempt.component.html'
})
export class ExemptComponent implements OnInit {
  newExempt:boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

}
