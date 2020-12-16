import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'area-home',
  template: `
  <county-list [type]="'area'" [areaId]="127"></county-list> 
  
  <p>

      area-home works!
    </p>
  `,
  styles: []
})
export class AreaHomeComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
