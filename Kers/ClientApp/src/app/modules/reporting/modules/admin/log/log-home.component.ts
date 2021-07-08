import { Component, OnInit } from '@angular/core';
import { Log, LogService } from './log.service';
import {ReportingService} from '../../../components/reporting/reporting.service';
import { Observable, Subject } from "rxjs";
import { FormGroup, FormBuilder } from "@angular/forms";
import { IMyDrpOptions } from "mydaterangepicker";
import { tap, startWith, debounceTime, flatMap, delay } from 'rxjs/operators';


@Component({
  templateUrl: 'log-home.component.html'
})
export class LogHomeComponent implements OnInit { 
    
    latest:Observable<Log[]>;
    numbLogs = 0;
    numLoaded = 10;
    pageSize = 10;
    errorMessage:string;
    condition = false;

    types:Observable<string[]>;

    numResults = 0;
    criteria = {
        search: '',
        rangeStart: '',
        rangeEnd: '',
        type: '',
        amount: 10
    }
    private searchTermStream = new Subject<string>();
    loading: boolean = true; // Turn spinner on and off


    private searchForm: FormGroup;
    private myDateRangePickerOptions: IMyDrpOptions = {
        // other options...
        dateFormat: 'dd.mm.yyyy',
        showClearBtn: false,
        showApplyBtn: false,
    };


    constructor( 
        private service:LogService,
        private reportingService: ReportingService,
        private formBuilder: FormBuilder 
    )   
    {
        this.latest = this.searchTermStream.asObservable()
                    .pipe(
                        startWith('onInit'),
                        debounceTime(300),
                        flatMap(_ => this.service.getCustom(this.criteria)),
                        //delay(1000),
                        tap(_ => this.loading = false)
                    );
    }

    ngOnInit(){

        this.defaultTitle();
        this.updateNumResults();

        this.types = this.service.types();

    }


    ngAfterViewInit(){
        this.searchTermStream.next("");
    }

    update(){
        this.loading = true;
        this.searchTermStream.next("");
    }

    

    onSearch(event){
        this.loading = true;
        //console.log(event);
        this.criteria.search = event.target.value;
        this.searchTermStream.next(event.target.value);
    }

    performSearch(term:string){
        this.loading = true;
        this.criteria.search = term;
        this.updateNumResults();
        this.searchTermStream.next('onRefresh'); // Emit value to force reload; actual value does not matter
    }

    updateNumResults(){
        this.service.getCustomCount(this.criteria).subscribe(
            num => {
                
                this.numbLogs = num;
                this.numResults = Math.min(num, this.criteria.amount);
                return this.numResults;
            },
            error => this.errorMessage = <any> error
        );
        
    }

    onType(event){
        this.loading = true;
        this.criteria.amount = 10;
        this.criteria.type = event.target.value;
        this.searchTermStream.next(this.criteria.search);
    }
    dateCnanged(event){
        // {beginDate: {year: 2018, month: 10, day: 9}, endDate: {year: 2018, month: 10, day: 19}}
        this.loading = true;
        var b = event.beginDate;
        this.criteria.rangeStart = b.month + '/'+ b.day +'/'+ b.year +' 00:00:00';
        var e = event.endDate;
        this.criteria.rangeEnd = e.month + '/'+ e.day +'/'+ e.year +' 00:00:00';
        this.criteria.amount = 10;
        this.searchTermStream.next(this.criteria.search);
    }

    defaultTitle(){
        this.reportingService.setTitle("System Logs");
    }

    loadMore(){
        this.criteria.amount += this.criteria.amount + this.pageSize;
        this.searchTermStream.next(this.criteria.search);
        this.updateNumResults();
    }
    

}