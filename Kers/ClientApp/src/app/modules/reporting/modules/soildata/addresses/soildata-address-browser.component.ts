import { Component, OnInit, Output, EventEmitter, Input} from '@angular/core';
import { FarmerAddress, SoildataService, FarmerAddressSearchResult } from '../soildata.service';
import { Observable, Subject } from 'rxjs';
import { FarmerAddressSearchCriteria } from '../soildata.report';
import { startWith, flatMap, tap } from 'rxjs/operators';

@Component({
  selector: 'soildata-address-browser',
  template: `
    <div *ngIf="close" class="ln_solid"></div>

    <div class="row" *ngIf="addresses$ | async as addresses">
      <div *ngIf="close" class="col-xs-12" style="margin-bottom: 30px;">
        <a class="btn btn-info btn-xs pull-right" (click)="canceled()">close</a>
      </div>
      <div class="col-sm-6 col-xs-12">
        <input type="text" [(ngModel)]="criteria.search" placeholder="Search by client name (first OR last)" (keyup)="onSearch($event)" class="form-control" name="Search" />
      </div>
      <div class="col-sm-6 col-xs-12 text-right">


        <div class="btn-group" data-toggle="buttons">
          <label class="btn btn-default" [class.active]="type=='frq'">
            <input type="radio" name="type" id="option2" (click)="switchOrder('frq')"> Use Frequency
          </label>
          <label class="btn btn-default" [class.active]="type=='asc'">
            <input type="radio" name="type" id="option3" (click)="switchOrder('asc')"> Name
          </label>
        </div>



      </div>
      <loading *ngIf="loading"></loading>
      <div *ngIf="!loading">
        <div class="col-lg-4 col-md-6 col-xs-12" *ngFor="let address of addresses.data"><br>
          <soildata-list-address [address]="address" [brief]="false"></soildata-list-address>
        <a class="btn btn-info btn-xs" (click)="selected(address)">select</a>
        </div>
      </div>


      <div class="col-xs-12"><br><br>
        <div *ngIf="addresses.count != 0" class="text-center">
          <div>Showing {{ (criteria.amount < addresses.count ? criteria.amount : addresses.count)}} of {{addresses.count}} Addresses</div>
          <div *ngIf="addresses.count >= criteria.amount" class="btn btn-app" style="width: 97%; margin-right: 35px;" (click)="loadMore()">
              load more <span class="glyphicon glyphicon-chevron-down" aria-hidden="true"></span>
        </div>
      </div><br><br>


    </div>

    <div class="row"><br>
      <div class="col-xs-12">
        <a class="btn btn-info btn-xs pull-right" (click)="newAddress = true" *ngIf="!newAddress">+ new address</a>
      </div>
      <div class="col-xs-12" *ngIf="newAddress">
        <div class="ln_solid"></div>
        <soildata-farmer-address-form (onFormCancel)="newAddress = false" (onFormSubmit)="selected($event)"></soildata-farmer-address-form>
      </div>
    </div>
    
    <div *ngIf="close" class="ln_solid"></div>
  `,
  styles: []
})
export class SoildataAddressBrowserComponent implements OnInit {


  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  type="frq";
  criteria:FarmerAddressSearchCriteria;
  

  @Input('close') close=true;


  
  @Output() onSelected = new EventEmitter<FarmerAddress>();
  @Output() onCanceled = new EventEmitter<void>();
  addresses$:Observable<FarmerAddressSearchResult>;
  newAddress = false;

  constructor(
    private service:SoildataService
  ) { }

  ngOnInit() {
    this.criteria = {
      search: "",
      order: 'frq',
      amount: 6
    }
    
    this.refresh = new Subject();

    this.addresses$ = this.refresh.asObservable()
      .pipe(
        startWith('onInit'), // Emit value to force load on page load; actual value does not matter
        flatMap(_ => this.service.getCustomAddresses(this.criteria)), // Get some items
        tap(_ => this.loading = false) // Turn off the spinner
      );
  }
  selected(address:FarmerAddress){
    this.onSelected.emit(address);
  }
  canceled(){
    this.onCanceled.emit();
  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }
  

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }
  
  switchOrder(type:string){
    this.type = type;
    this.criteria["order"] = type;
    this.onRefresh();
  }

  loadMore(){
    this.criteria.amount += 6;
    this.onRefresh();
  }

}