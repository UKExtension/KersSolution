import { Component, Input, OnInit } from '@angular/core';
import { ActivitySignUpEntry } from './signup.service';

@Component({
  selector: '[signup-list-row]',
  template: `
  <td>{{attendie.name}}</td>
  <td>{{attendie.address}}</td>
  <td>{{attendie.email}}</td>
  <td class="text-right">
  
    <a class="btn btn-info btn-xs" ><i class="fa fa-pencil"></i></a>
    <a class="btn btn-info btn-xs"><i class="fa fa-trash-o"></i></a>
  
  </td>
  `,
  styles: [
  ]
})
export class SignupListRowComponent implements OnInit {
  @Input('signup-list-row') attendie:ActivitySignUpEntry = null;

  constructor() { }

  ngOnInit(): void {
  }

}
