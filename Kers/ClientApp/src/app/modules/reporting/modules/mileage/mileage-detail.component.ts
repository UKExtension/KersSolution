import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Mileage } from './mileage';
import { MileageService } from './mileage.service';

@Component({
  selector: 'mileage-detail',
  template: `
  <div class="ln_solid"></div>
  <div class="row">
      <div class="col-xs-9">
          
          <article class="media event" *ngIf="rowDefault">
              <a class="pull-left date">
                  <p class="day">{{expense.expenseDate | date:'d'}}</p>   
                  <p class="month">{{expense.expenseDate | date:'MMM'}} </p>
              </a>
              <div class="media-body">
              <a class="title">{{location}}</a>
              <p>Mileage: {{miles}}</p>
              </div>
          </article>
          <div class="col-xs-12" *ngIf="rowEdit">
              <mileage-form *ngIf="isItMileageRecord" [mileage]="expense" (onFormCancel)="default()" (onFormSubmit)="expenseSubmitted($event)"></mileage-form>
              <expense-compatability-form *ngIf="!isItMileageRecord" [expense]="expense" (onFormCancel)="default()" (onFormSubmit)="expenseSubmitted($event)"></expense-compatability-form>
          </div>
          <div class="col-xs-11" *ngIf="rowDelete">
              Do you really want to delete expense <strong>{{expense.expenseLocation}}</strong>?<br><button (click)="confirmDelete()" class="btn btn-info btn-xs">Yes</button> <button (click)="default()" class="btn btn-info btn-xs">No</button>
          </div>
          
      </div>
      <div class="col-xs-3 text-right">
          <a class="btn btn-info btn-xs" (click)="edit()" *ngIf="rowDefault">edit</a>
          <a class="btn btn-info btn-xs" (click)="delete()" *ngIf="rowDefault">delete</a>
          <a class="btn btn-info btn-xs" (click)="default()" *ngIf="!rowDefault">close</a>
      </div>  
  </div>
  
  `,
  styles: []
})
export class MileageDetailComponent implements OnInit {

  rowDefault =true;
  rowEdit = false;
  rowDelete = false;

  isItMileageRecord = true;
  miles:number = 0;
  location:string = "";
  
  @Input() expense:Mileage;

  @Output() onDeleted = new EventEmitter<Mileage>();
  @Output() onEdited = new EventEmitter<Mileage>();
  
  errorMessage: string;

  constructor( 
      private service:MileageService
  )   
  {}

  ngOnInit(){ 
    this.isItMileageRecord = (this.expense.segments != undefined && this.expense.segments.length != 0);
    this.miles = this.isItMileageRecord ? this.expense.segments.map(item => item.mileage).reduce((prev, next) => prev + next) : this.expense.mileage;
    this.location = this.expense.expenseLocation;
    if( this.isItMileageRecord ){
      var firstDestination = this.expense.segments[0].location;
      this.location = firstDestination.displayName;
      this.location += (this.location.length == 0 ? "":', ') + firstDestination.address.building;
      this.location += ", " + firstDestination.address.city + ", " + firstDestination.address.state;
    }
     
  }
  edit(){
      this.rowDefault = false;
      this.rowEdit = true;
      this.rowDelete = false;
  }
  delete(){
      this.rowDefault = false;
      this.rowEdit = false;
      this.rowDelete = true;
  }
  default(){
      this.rowDefault = true;
      this.rowEdit = false;
      this.rowDelete = false;
  }

  expenseSubmitted(expense:Mileage){
      this.expense = expense;
      this.onEdited.emit(expense);
      this.default();
  }

  confirmDelete(){
      
      this.service.delete(this.expense.id).subscribe(
          res=>{
              this.onDeleted.emit(this.expense);
          },
          err => this.errorMessage = <any> err
      );
      
  }

}
