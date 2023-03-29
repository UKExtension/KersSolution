import { Component, OnInit, Input } from '@angular/core';
import { SoilReportSearchCriteria, SoilReportBundle, TypeForm, SoilReportStatus } from '../soildata.report';
import { Subject, Observable } from 'rxjs';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { SoildataService } from '../soildata.service';
import { startWith, tap, mergeMap, map, scan } from 'rxjs/operators';
import { saveAs } from 'file-saver';

@Component({
  selector: 'soildata-reports-catalog',
  templateUrl: 'soildata-reports-catalog.component.html'
})
export class SoildataReportsCatalogComponent implements OnInit {
  refresh: Subject<SoilReportBundle[] | null>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  reports$:Observable<SoilReportBundle[]>;
  filteredReports:SoilReportBundle[];
  reportsByDateRange: SoilReportBundle[];
  type="dsc";
  pdfLoading = false;
  samplePdfLoading = false;
  csvDataLoading = false;

  reportForCopy:SoilReportBundle = null;
  isThisSampleCopy:boolean = false;

  reportsExist = false;
  samplesExist = false;

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
  availableStatuses:SoilReportStatus[] = [];

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
    this.getStatuses();
    this.getFormTypes();
    this.service.getCustom(this.criteria).subscribe(
      res =>{
        this.reportsByDateRange = res;
        this.filteredReports = this.reportsByDateRange;
        this.loading = false;
      } 
    )


    /* 
    this.reports$ = this.refresh.asObservable()
      .pipe(
        startWith({} as SoilReportBundle[]), // Emit value to force load on page load; actual value does not matter
        mergeMap(initialItemState =>  this.service.getCustom(this.criteria)), // Get some items
        tap(_ => this.loading = false) // Turn off the spinner
      );
     */
  }


  dateCnanged(event: IMyDateModel){
    this.startDate = event.dateRange.beginJsDate;
    this.endDate = event.dateRange.endJsDate;
    this.criteria["start"] = event.dateRange.beginJsDate.toISOString();
    this.criteria["end"] = event.dateRange.endJsDate.toISOString();
    this.criteria.status = [];
    this.criteria.formType = [];
    this.onRefresh();
    this.getStatuses();
    this.getFormTypes();
  }

  applyFilterCriteria(){
    //Apply form type checkboxes
    this.filteredReports = this.reportsByDateRange.filter( r => this.criteria.formType.includes(r.typeForm.id));
    //Filter by status
    this.filteredReports = this.filteredReports.filter( r => this.criteria.status.includes( r.lastStatus.soilReportStatusId));
    //Filter by name
    if(this.criteria.search != undefined && this.criteria.search != ""){
      this.filteredReports = this.filteredReports.filter( r => (
        r.farmerForReport.first.toLowerCase().includes(this.criteria.search.toLowerCase())
        ||
        r.farmerForReport.last.toLowerCase().includes(this.criteria.search.toLowerCase())
         ));
    }
    //Apply Sorting
    if(this.criteria.order == 'dsc'){
      this.filteredReports.sort((a,b) => ((new Date(b.sampleLabelCreated).getTime()) - ( new Date(a.sampleLabelCreated).getTime())));
    }else if(this.criteria.order == 'asc'){
      this.filteredReports.sort((a,b)=> ((new Date(a.sampleLabelCreated).getTime()) - ( new Date(b.sampleLabelCreated).getTime())));
    }else if(this.criteria.order == "smplasc"){
      this.filteredReports.sort((a,b)=> (a.coSamnum.padStart(6, '0') > b.coSamnum.padStart(6, '0')) ? 1 : -1);
    }else if(this.criteria.order == "smpl"){
      this.filteredReports.sort((a,b)=> (b.coSamnum.padStart(6, '0') > a.coSamnum.padStart(6, '0')) ? 1 : -1);
    }
    this.softUpdateIfSampleExists();
  }

  getStatuses(){
    this.statusesCheckboxes = [];
    this.criteria.status = [];
    this.service.getCustomStatuses(this.criteria).subscribe(
      res => {
        this.availableStatuses = res;
        for(let status of res){
          this.criteria.status.push(status.id);
          this.statusesCheckboxes.push({
            name:status.name, value: status.id, checked:true
          })
        }
      }
    )

  }
  getFormTypes(){
    this.typesCheckboxes = [];
    this.service.getCustomFormTypes(this.criteria).subscribe(
      res => {
        for(let type of res){
          this.criteria.formType.push(type.id);
          this.typesCheckboxes.push({
            name:type.code, value: type.id, checked:true
          })
        }
      }
    )
  }
  softUpdateAvailableStatuses(){
    var oldStatuses:SoilReportStatus[] = [];
    this.availableStatuses.forEach(val => oldStatuses.push(Object.assign({}, val)));
    this.availableStatuses = [];
    for( let rep of  this.reportsByDateRange){
      if( !this.availableStatuses.map(s => s.id).includes(rep.lastStatus.soilReportStatusId)) this.availableStatuses.push(rep.lastStatus.soilReportStatus);
    }
    
    //this.criteria.status = this.availableStatuses.map( s => s.id);
    this.statusesCheckboxes = [];
    for(let status of this.availableStatuses){
      console.log(status.id);
      console.log( this.criteria.status);
      if( this.criteria.status.includes(status.id) ){
        console.log('status matches');
        this.statusesCheckboxes.push({
          name:status.name, value: status.id, checked:true
        });
      }else if(!oldStatuses.map(s => s.id).includes(status.id)){
        console.log('new status');
        this.criteria.status.push(status.id)
        this.statusesCheckboxes.push({
          name:status.name, value: status.id, checked:true
        });
      }else{
        console.log( "not checked")
        this.statusesCheckboxes.push({
          name:status.name, value: status.id, checked:false
        });
      }
    }
    console.log(this.statusesCheckboxes);
  }

  softUpdateIfSampleExists(){
    this.samplesExist = this.filteredReports.filter( r => r.lastStatus.soilReportStatus.name == "Entered").length != 0;
  }
  softUpdateIfReportsExists(){
    this.samplesExist = this.filteredReports.filter( r => r.reports != undefined && r.reports.length != 0).length != 0;
  }



  statusChanged(){
    this.getStatuses();
    this.onRefresh();
  }
  reportAgentNoteChanged(event:SoilReportStatus){
    this.softUpdateAvailableStatuses();
    this.applyFilterCriteria();
/* 

    if( this.availableStatuses.filter( s => s.id == event.id).length == 0 ){
      this.service.reportStatuses().subscribe(
        res => {
          var enteredStatus = res.filter( s => s.id == event.id);
          if(enteredStatus.length > 0 ){
            var entered = enteredStatus[0];
            this.criteria.status.push(entered.id);
            this.availableStatuses.push(entered);
            this.statusesCheckboxes.push({
              name:entered.name, value: entered.id, checked:true
            })
          }
        }
      )
    }

 */

  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.applyFilterCriteria();
    //this.onRefresh();
  }
  onFormTypeChange(){
    this.criteria.formType = this.selectedFormTypes;
    this.applyFilterCriteria();
  }

  onReportStatusesChange(){
    this.criteria.status = this.selectedReportStatuses;
    //this.getStatuses();
    this.applyFilterCriteria();
  }

  onRefresh() {
    this.reportsExist = false;
    this.samplesExist = false;
    this.loading = true; // Turn on the spinner.
    this.service.getCustom(this.criteria).subscribe(
      res =>{
        this.reportsByDateRange = res;
        this.filteredReports = this.reportsByDateRange;
        this.loading = false;
      } 
    )
  }

  SampleFormCanceled(){
    this.newSample = false;
  }
  SampleFormSubmit(event:SoilReportBundle){
    this.newSample = false;
    this.lastCountyNumber = event.coSamnum;
    this.sampleNumberDisplayed = true;
    this.reportsByDateRange.unshift(event);
    if(this.filteredReports.filter( f => f.id == event.id).length == 0 ) this.filteredReports.unshift(event);
    this.softUpdateAvailableStatuses();
    this.softUpdateIfSampleExists();
  }
  
  switchOrder(type:string){
    this.type = type;
    this.criteria["order"] = type;
    this.applyFilterCriteria();
    //this.onRefresh();
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
  registerReports(event:boolean){
    if( event ) this.reportsExist = true;
  }
  registerSamples(event:boolean){
    if( event ) this.samplesExist = true;
  }

  printAll(reports:SoilReportBundle[]){
    this.pdfLoading = true;
    this.service.consolidatedPdf(reports.map(r => r.uniqueCode)).subscribe(
      data => {
        var blob = new Blob([data], {type: 'application/pdf'});
        saveAs(blob, "SoilTestResults.pdf");
        this.pdfLoading = false;
        this.service.reportStatuses().subscribe(
          res => {
            var reviewedStatus = res.filter( s => s.name == "Reviewed")[0];
            var archivedStatus = res.filter( s => s.name == "Archived")[0];
            for( var rep of this.filteredReports){
              if(rep.lastStatus.soilReportStatusId == reviewedStatus.id){
                rep.lastStatus.soilReportStatus = archivedStatus;
                rep.lastStatus.soilReportStatusId = archivedStatus.id;
              }
            }
            this.softUpdateAvailableStatuses();
            this.applyFilterCriteria();
            this.softUpdateIfReportsExists;
          }
        )
      }
    )
  }

  printPackingSlip(reports:SoilReportBundle[]){
    this.samplePdfLoading = true;
    this.service.packingSlipdPdf(reports.map(r => r.uniqueCode)).subscribe(
      data => {
        var blob = new Blob([data], {type: 'application/pdf'});
        saveAs(blob, "PackingSlip.pdf");
        this.samplePdfLoading = false;

        this.service.reportStatuses().subscribe(
          res => {
            var enteredStatus = res.filter( s => s.name == "Entered")[0];
            var sentStatus = res.filter( s => s.name == "Sent")[0];
            for( var rep of this.filteredReports){
              if(rep.lastStatus.soilReportStatusId == enteredStatus.id){
                rep.lastStatus.soilReportStatus = sentStatus;
                rep.lastStatus.soilReportStatusId = sentStatus.id;
              }
            }
            this.softUpdateAvailableStatuses();
            this.applyFilterCriteria();
            this.softUpdateIfSampleExists();
          }
        )
      }
    )
  }
  downloadCsv(reports:SoilReportBundle[]) {
    this.csvDataLoading = true;
    var ids = reports.map(r => r.uniqueCode);
    console.log(ids);
    this.service.getData(ids).subscribe(
      data => {
        const replacer = (key, value) => value === null ? '' : value; // specify how you want to handle null values here
        const header = Object.keys(data[0]);
        let csv = data.map(row => header.map(fieldName => JSON.stringify(row[fieldName], replacer)).join(','));
        let csvArray = csv.join('\r\n');

        var blob = new Blob([csvArray], {type: 'text/csv' })
        saveAs(blob, "KERS_SoilDataReports.csv");


        this.csvDataLoading = false;
      }
    )


    
  }


}
