import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Mileage, MileageMonth } from './mileage';

@Component({
  selector: 'mileage-month',
  template: `
  <h3>{{month.date | date:'MMMM, y'}}</h3>
  <mileage-detail *ngFor="let expense of month.expenses" [expense]="expense" (onDeleted)="deleted($event)" (onEdited)="edit($event)"></mileage-detail><br><br>
  `,
  styles: []
})
export class MileageMonthComponent implements OnInit {
  @Input() month:MileageMonth;
  @Output() onDeleted = new EventEmitter<Mileage>();
  @Output() onEdited = new EventEmitter<Mileage>();

  constructor() { }

  ngOnInit() {
  }

  deleted(expense:Mileage){
    this.onDeleted.emit(expense);
  }
  edit(expense:Mileage){
      this.onEdited.emit(expense)
  }

}
