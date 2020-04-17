import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FiscalYear, FiscalyearService } from '../../modules/admin/fiscalyear/fiscalyear.service';

@Component({
  selector: 'fiscal-year-switcher',
  template: `
  <div class="row" *ngIf="fiscalYears != null && fiscalYears.length > 1 && selectedFiscalYear != null">
    <div class="col-md-5">
      <span *ngIf="isItFiscal">Fiscal </span>Year: <span *ngFor="let year of fiscalYears"><a (click)="selectFiscalYear(year)" [class.active-year]="year.id == selectedFiscalYear.id" style="cursor:pointer;">{{year.name}}</a> | </span>
    </div>
  </div>
  `,
  styles: [`
  .active-year{
      font-weight: bold;
  }
`]
})
export class FiscalYearSwitcherComponent implements OnInit {

  @Input() initially = "next"; // next, current, previous
  @Input() type = "serviceLog";
  @Input() showNext = true;
  @Input() isItFiscal = true;
  @Output() onLoaded = new EventEmitter<FiscalYear[]>();
  @Output() onSwitched = new EventEmitter<FiscalYear>();




  fiscalYears: FiscalYear[];
  nextFiscalYear: FiscalYear;
  previousFiscalYear: FiscalYear;
  currentFiscalYear: FiscalYear;
  selectedFiscalYear: FiscalYear;

  constructor(
    private fiscalYearService: FiscalyearService
  ) { }

  ngOnInit() {
    if( this.initially == "previous"){
      this.getPreviousFiscalYear();
    }else{
      this.getNextFiscalYear();
    }
    
  }
  getFiscalYears(){
    this.fiscalYearService.byType(this.type).subscribe(
        res => {
            this.fiscalYears = <FiscalYear[]>res;
            
            if(!this.showNext){
              var now = new Date();
              this.fiscalYears = this.fiscalYears.filter( y => new Date(y.start) < now);
            }
            if(!this.isItFiscal){
              this.fiscalYears.unshift( <FiscalYear>{name:"2017", id:12345});
              this.fiscalYears.unshift( <FiscalYear>{name:"2016", id:12346});
            }
            this.onLoaded.emit(this.fiscalYears);
        }
    );
  }
  getNextFiscalYear(){
    this.fiscalYearService.next(this.type).subscribe(
        res => {
            this.nextFiscalYear = res;
            this.getCurrentFiscalYear();
            
        }
    );
  }
  getPreviousFiscalYear(){
    this.fiscalYearService.previous(this.type).subscribe(
        res => {
            this.previousFiscalYear = res;
            this.getCurrentFiscalYear();
            
        }
    );
  }
  getCurrentFiscalYear(){
      this.fiscalYearService.current(this.type).subscribe(
          res => {
              this.currentFiscalYear = res;
              if(this.initially == "next"){
                this.selectFiscalYear(this.nextFiscalYear);
              }else if(this.initially == "previous"){
                this.selectFiscalYear(this.previousFiscalYear);
              }else{
                this.selectFiscalYear(this.currentFiscalYear);
              }
              this.getFiscalYears();
          }
      );
  }

  
  selectFiscalYear(year:FiscalYear){
      if(this.selectedFiscalYear == null){
        this.selectedFiscalYear = year;
        this.onSwitched.emit(this.selectedFiscalYear);
      }else if(this.selectedFiscalYear.id != year.id){
        this.selectedFiscalYear = year;
        this.onSwitched.emit(this.selectedFiscalYear);
      }
      
  }
}
