import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'area-home',
  template: `
  <county-list [type]="'area'" [areaId]="0"></county-list> 
  
  <p>

      area-home works!
    </p>
  `,
  styles: []
})
export class AreaHomeComponent implements OnInit {

  @Input() areaId:number;

  constructor() { }

  ngOnInit() {
  }

}
