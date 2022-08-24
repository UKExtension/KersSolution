import { Component, OnInit, Input } from '@angular/core';
import { SoilReportSearchCriteria, SoilReportBundle, TypeForm } from '../soildata.report';
import { Subject, Observable } from 'rxjs';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { SoildataService } from '../soildata.service';
import { startWith, tap, mergeMap } from 'rxjs/operators';
import { saveAs } from 'file-saver';

@Component({
  selector: 'soildata-reports-catalog',
  templateUrl: 'soildata-reports-catalog.component.html'
})
export class SoildataReportsCatalogComponent implements OnInit {
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  reports$:Observable<SoilReportBundle[]>;
  type="dsc";
  pdfLoading = false;

  reportForCopy:SoilReportBundle = null;
  isThisSampleCopy:boolean = false;

  @Input() criteria:SoilReportSearchCriteria;
  @Input() startDate:Date;
  @Input() endDate:Date;



  condition = false;

  newSample:boolean = false;
  sampleNumberDisplayed:boolean = false;
  lastCountyNumber:string;


  myDateRangePickerOptions: IAngularMyDpOptions = {
    dateRange: true,
    dateFormat: 'mmm dd, yyyy',
    alignSelectorRight: true
  };
  model:IMyDateModel = null;


  typesCheckboxes = [];
  statusesCheckboxes = [];

  get selectedFormTypes() { 
    return this.typesCheckboxes
              .filter(opt => opt.checked)
              .map(opt => opt.value)
  }

  get selectedReportStatuses() { 
    return this.statusesCheckboxes
              .filter(opt => opt.checked)
              .map(opt => opt.value)
  }

  constructor(
    private service:SoildataService
  ) { }

  ngOnInit() {
    if( this.startDate == null){
      this.startDate = new Date();
      this.startDate.setMonth( this.startDate.getMonth() -2);
    }
    if( this.endDate == null ){
      this.endDate = new Date();
    }
    this.service.formTypes().subscribe(
      res => {
        for(let type of res){
          this.criteria.formType.push(type.id);
          this.typesCheckboxes.push({
            name:type.code, value: type.id, checked:true
          })
        }
      }
    )

    this.service.reportStatuses().subscribe(
      res => {
        for(let status of res){
          this.criteria.status.push(status.id);
          this.statusesCheckboxes.push({
            name:status.name, value: status.id, checked:true
          })
        }
      }
    )
    
    this.criteria = {
      start: this.startDate.toISOString(),
      end: this.endDate.toISOString(),
      search: "",
      order: 'dsc',
      status: [],
      formType: []
    }


    this.model = {
      isRange: true, 
      singleDate: null, 
      dateRange: {
        beginDate: {
          year: this.startDate.getFullYear(), month: this.startDate.getMonth() + 1, day: this.startDate.getDate()
        },
        endDate: {
          year: this.endDate.getFullYear(), month: this.endDate.getMonth() + 1, day: this.endDate.getDate()
        }
      }
    }
    this.refresh = new Subject();

    this.reports$ = this.refresh.asObservable()
      .pipe(
        startWith('onInit'), // Emit value to force load on page load; actual value does not matter
        mergeMap(_ => this.service.getCustom(this.criteria)), // Get some items
        tap(_ => this.loading = false) // Turn off the spinner
      );
  }


  dateCnanged(event: IMyDateModel){
    this.startDate = event.dateRange.beginJsDate;
    this.endDate = event.dateRange.endJsDate;
    this.criteria["start"] = event.dateRange.beginJsDate.toISOString();
    this.criteria["end"] = event.dateRange.endJsDate.toISOString();
    this.onRefresh();
  }

  statusChanged(){
    this.onRefresh();
  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }
  onFormTypeChange(){
    this.criteria.formType = this.selectedFormTypes;
    this.onRefresh();
  }

  onReportStatusesChange(){
    this.criteria.status = this.selectedReportStatuses;
    this.onRefresh();
  }

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }

  SampleFormCanceled(){
    this.newSample = false;
  }
  SampleFormSubmit(event:SoilReportBundle){
    this.newSample = false;
    this.lastCountyNumber = event.coSamnum;
    this.sampleNumberDisplayed = true;
    var ths = this;
    setTimeout(()=>{  ths.onRefresh();    }, 40);
    
  }
  
  switchOrder(type:string){
    this.type = type;
    this.criteria["order"] = type;
    this.onRefresh();
  }

  copySample(event:SoilReportBundle){
    this.reportForCopy = event;
    this.isThisSampleCopy = true;
    this.newSample = true;
    setTimeout(()=>{  
      this.reportForCopy = null;
      this.isThisSampleCopy = false;
          }, 400);
    
  }

  printAll(reports:SoilReportBundle[]){
    this.pdfLoading = true;
    this.service.consolidatedPdf(reports.map(r => r.uniqueCode)).subscribe(
      data => {
        var blob = new Blob([data], {type: 'application/pdf'});
        saveAs(blob, "SoilTestResults.pdf");
        this.pdfLoading = false;
        this.onRefresh();
      }
    )
    

  }


}
