import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { ReportingService } from '../../components/reporting/reporting.service';
import { Mileage, MileageMonth } from './mileage';
import { MileageService } from './mileage.service';

@Component({
  selector: 'app-mileage-home',
  template: `

  <div>
    <div class="text-right">
        <a class="btn btn-info btn-xs" *ngIf="!newExpense" (click)="newExpense = true">+ new mileage record</a>
    </div>
    <mileage-form *ngIf="newExpense" [isNewCountyVehicle]="newCountyVehicle" (onFormCancel)="newExpense=false" (onFormSubmit)="newExpenseSubmitted($event)"></mileage-form>
  </div><br><br>
  <mileage-month *ngFor="let mnth of byMonth" [month]="mnth" (onEdited)="edited($event)" (onDeleted)="deleted($event)"></mileage-month><br><br>
  <div *ngIf="numbExpenses != 0" class="text-center">
        <div>Showing {{latest.length}} of {{numbExpenses}} expense records</div>
        <div *ngIf="latest.length < numbExpenses" class="btn btn-app" style="width: 97%; margin-right: 35px;" (click)="loadMore()">
            load more <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
        </div>
    </div>
  `,
  styles: []
})
export class MileageHomeComponent implements OnInit {
  latest:Mileage[] = [];
  numbExpenses:number = 0;
  newExpense=false;
  byMonth:MileageMonth[] = [];
  errorMessage: string;
  newCountyVehicle = false;


  constructor( 
      private reportingService: ReportingService,
      private route: ActivatedRoute,
      private service:MileageService
  )   
  {}

  ngOnInit(){
      
      this.defaultTitle();
      this.service.latest().subscribe(
          res=>{
                  this.latest = <Mileage[]>res; 
                  this.populateByMonth();
              },
          err => this.errorMessage = <any>err
      );
      this.service.num().subscribe(
          res => {
              this.numbExpenses = <number>res;
          },
          err => this.errorMessage = <any>err
      );
      this.route.params
      .subscribe( (params: Params) =>
          {
              var type = params['type'];
              if(type != undefined){
                  if(type == 'new'){
                      this.newExpense = true;
                  }else if(type = 'newcountyvehicle'){
                      this.newExpense = true;
                      this.newCountyVehicle = true;
                  }
              }
          }
      );
      
  }
  loadMore(){
      var lt = this.latest;
      this.service.latest(this.latest.length, 2).subscribe(
          res=>{
                  var batch = <Mileage[]>res; 
                  batch.forEach(function(element){
                      lt.push(element);
                  });
                  this.byMonth = [];
                  this.populateByMonth();
              },
          err => this.errorMessage = <any>err
      );
  }
  populateByMonth(){
      var bm = this.byMonth;
      this.latest.forEach(function(element){
          
              var exDt = new Date(element.expenseDate);
              var exMonth = bm.filter(f=>f.month==exDt.getMonth() && f.year == exDt.getFullYear());
              if(exMonth.length == 0){
                  var ob = <MileageMonth>{
                      month : exDt.getMonth(),
                      year : exDt.getFullYear(),
                      date: exDt,
                      expenses : [element]
                  };
                  bm.push(ob);
              }else{
                  exMonth[0].expenses.push(element);
              }
          });
  }


  newExpenseSubmitted(expense:Mileage){
      this.newExpense=false;
      this.latest.unshift(expense);
      this.byMonth = [];
      this.populateByMonth();
      this.numbExpenses++;
  }
  
  deleted(expense:Mileage){
      let index: number = this.latest.indexOf(expense);
      if (index !== -1) {
          this.latest.splice(index, 1);
          this.numbExpenses--;
      }
      this.byMonth = [];
      this.populateByMonth();
  }

  edited(expense:Mileage){

      this.latest = this.latest.map(function(item) { return item.expenseId == expense.expenseId ? expense : item; });
      this.latest.sort(
                  function(a, b) {

                      var dateA = new Date(a.expenseDate);
                      var dateB = new Date(b.expenseDate);
                      if( dateA  > dateB ){
                          return -1;
                      }else{
                          return 1;
                      }
                  }
               );
      this.byMonth = [];
      this.populateByMonth();
  }

  defaultTitle(){
      this.reportingService.setTitle("Mileage Records");
  }
}
