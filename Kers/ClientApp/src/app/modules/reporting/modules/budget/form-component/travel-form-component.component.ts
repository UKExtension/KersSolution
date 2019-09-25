import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'travel-form-component',
  templateUrl: './travel-form-component.component.html',
  styles: []
})
export class TravelFormComponentComponent implements OnInit {

  @Input() budgetForm:FormGroup;


  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit() {
  }

}
