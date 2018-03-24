import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FiscalYear, FiscalyearService } from '../../modules/admin/fiscalyear/fiscalyear.service';

@Component({
  selector: 'fiscal-year-switcher',
  template: `
  <div class="row" *ngIf="fiscalYears != null && selectedFiscalYear != null">
    <div class="col-md-5">
      Fiscal Year: <span *ngFor="let year of fiscalYears"><a (click)="selectFiscalYear(year)" [class.active-year]="year.id == selectedFiscalYear.id" style="cursor:pointer;">{{year.name}}</a> | </span>
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

  @Input() initially = "next"; // next or current
  @Output() onLoaded = new EventEmitter<FiscalYear[]>();
  @Output() onSwitched = new EventEmitter<FiscalYear>();




  fiscalYears: FiscalYear[];
  nextFiscalYear: FiscalYear;
  currentFiscalYear: FiscalYear;
  selectedFiscalYear: FiscalYear;

  constructor(
    private fiscalYearService: FiscalyearService
  ) { }

  ngOnInit() {
    this.getNextFiscalYear();
  }
  getFiscalYears(){
    this.fiscalYearService.byType("serviceLog").subscribe(
        res => {
            this.fiscalYears = res;
            this.onLoaded.emit(this.fiscalYears);
        }
    );
  }
  getNextFiscalYear(){
    this.fiscalYearService.next("serviceLog").subscribe(
        res => {
            this.nextFiscalYear = res;
            this.getCurrentFiscalYear();
            
        }
    );
  }
  getCurrentFiscalYear(){
      this.fiscalYearService.current("serviceLog").subscribe(
          res => {
              this.currentFiscalYear = res;
              if(this.initially == "next"){
                this.selectFiscalYear(this.nextFiscalYear);
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
