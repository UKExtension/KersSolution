import { Component, Input, OnInit } from '@angular/core';
import { ActivitySignUpEntry } from './signup.service';

@Component({
  selector: '[signup-list-row]',
  template: `
  <td *ngIf="defaultView">{{attendie.name}}</td>
  <td *ngIf="defaultView">{{attendie.address}}</td>
  <td *ngIf="defaultView">{{attendie.email}}</td>
  <td class="text-right" *ngIf="defaultView">
  
    <a class="btn btn-info btn-xs" ><i class="fa fa-pencil"></i></a>
    <a class="btn btn-info btn-xs"><i class="fa fa-trash-o"></i></a>
  
  </td>
  <td *ngIf="editView">
  edit
  </td>
  <td *ngIf="deleteView">
  delete
  </td>
  `,
  styles: [
  ]
})
export class SignupListRowComponent implements OnInit {
  @Input('signup-list-row') attendie:ActivitySignUpEntry = null;
  defaultView = true;
  editView = false;
  deleteView = false;

  constructor() { }

  ngOnInit(): void {
  }

}
