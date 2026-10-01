import { Component, EventEmitter, Input, OnInit, Output, ChangeDetectionStrategy } from '@angular/core';
import { Mileage, MileageMonth } from './mileage';

@Component({
    selector: 'mileage-month',
    template: `
  <h3>{{month.date | date:'MMMM, y'}}</h3>
  @for (expense of month.expenses; track expense) {
    <mileage-detail [expense]="expense" (onDeleted)="deleted($event)" (onEdited)="edit($event)"></mileage-detail>
    }<br><br>
  `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
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
