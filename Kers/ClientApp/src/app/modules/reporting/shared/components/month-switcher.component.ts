import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'month-switcher',
  template: `
  <div class="row">
    <div class="col-md-5">
      Month: <span *ngFor="let month of months"><a (click)="selectMonth(month)" [class.active-month]="month.getMonth() == selectedMonth.getMonth()" style="cursor:pointer;">{{month | date:'MMM yyyy'}}</a> | </span>
    </div>
  </div>
  `,
  styles: [`
  .active-month{
      font-weight: bold;
  }
`]
})
export class MonthSwitcherComponent implements OnInit {

  @Input() initially = "previous"; // next, current, previous
  @Input() numMonths = 4;
  @Input() showNext = false;
  @Output() onLoaded = new EventEmitter<Date>();
  @Output() onSwitched = new EventEmitter<Date>();

  months:Date[] = [];

  selectedMonth:Date = new Date();

  constructor(
  ) {

    var now = new Date();
    var start = (this.showNext ? -1 : 0 );
    for(var i = start; i < this.numMonths; i++){
        var running = new Date();
        running.setMonth(now.getMonth() - i );
        this.months.push( running );
    }
    this.months = this.months.reverse();
 }

  ngOnInit() {
    var current = this.months[this.months.length - 1]
    if( this.initially == "previous") current = this.months[this.months.length - 2];
    this.selectMonth( current);  
    this.onLoaded.emit(this.months[this.months.length - 1]); 
  }
  
  getCurrentMonth(){
      
  }

  
  selectMonth(month:Date){
    this.selectedMonth = month;
    this.onSwitched.emit(month);
  }
}
