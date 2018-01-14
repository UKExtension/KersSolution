import { Component, OnInit } from '@angular/core';
import { Log, LogService } from './log.service';
import {ReportingService} from '../../../components/reporting/reporting.service';
import { Observable } from "rxjs/Observable";
import { Subject } from "rxjs/Subject";
import { FormGroup, FormBuilder } from "@angular/forms";
import { IMyDrpOptions } from "mydaterangepicker";


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
        this.latest = this.searchTermStream
                        .debounceTime(300)
                        .switchMap((term:string) => {
                            return this.performSearch(term);
                        });
    }

    ngOnInit(){

        this.defaultTitle();
        this.updateNumResults();

        this.types = this.service.types();

    }


    ngAfterViewInit(){
        this.searchTermStream.next("");
    }

    

    onSearch(event){
         this.searchTermStream.next(event.target.value);
    }

    performSearch(term:string){
        this.criteria.search = term;
        this.updateNumResults();
        return this.service.getCustom(this.criteria);
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
 
            this.criteria.amount = 10;
            this.criteria.type = event.target.value;
            this.searchTermStream.next(this.criteria.search);
        
    }
    dateCnanged(event){
        // {beginDate: {year: 2018, month: 10, day: 9}, endDate: {year: 2018, month: 10, day: 19}}
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