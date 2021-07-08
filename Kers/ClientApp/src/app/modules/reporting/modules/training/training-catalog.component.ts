import { Component, OnInit, Input } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { Training, TrainingSearchCriteria } from './training';
import { TrainingService } from './training.service';
import { startWith, flatMap, delay, map, tap } from 'rxjs/operators';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';

@Component({
  selector: 'app-training-catalog',
  templateUrl: './training-catalog.component.html',
  styles: []
})
export class TrainingCatalogComponent implements OnInit {
  refresh: Subject<string>; // For load/reload
  loading: boolean = true; // Turn spinner on and off
  trainings$:Observable<Training[]>;
  type="dsc";

  @Input() criteria:TrainingSearchCriteria;
  @Input() admin:boolean = false;
  @Input() attendance:boolean = false;
  @Input() startDate:Date;
  @Input() endDate:Date;

  condition = false;


  myDateRangePickerOptions: IAngularMyDpOptions = {
      dateRange: true,
      dateFormat: 'mmm dd, yyyy'
  };
  model: IMyDateModel = null;

  constructor(
    private service:TrainingService,
    private route: ActivatedRoute
  ) { 
    //this.trainings = service.range();
  }

  ngOnInit() {
    this.route.paramMap.subscribe(
      (params: ParamMap) =>{



        if(this.criteria == null){
          //Criterias are NOT passed to the component

          if(params.has("start")){
            //Criterias are passed to the ROUTE
            this.criteria = <TrainingSearchCriteria>{
              start: <string> params.get("start"),
              end: <string> params.get("end"),
              search: <string> params.get("search"),
              contacts: <string> params.get("contacts"),
              day: params.get("day") == "null" ? null : +params.get("day"),
              withseats: <boolean> <unknown>params.get("withseats"),
              order: <string> params.get("order"),
              attendance: this.attendance,
              admin: this.admin
            }
            this.startDate = new Date(this.criteria.start);
            this.endDate = new Date(this.criteria.end);
          }else{
            //Criterias are NOT passed
            if( this.startDate == null){
              this.startDate = new Date();
            }
            if( this.endDate == null ){
              this.endDate = new Date();
              this.endDate.setMonth( this.endDate.getMonth() + 5);
            }
            
            this.criteria = {
              start: this.startDate.toISOString(),
              end: this.endDate.toISOString(),
              search: "",
              status: this.admin ? 'all' : 'published',
              contacts: "",
              day: null,
              order: 'dsc',
              withseats: false,
              attendance: this.attendance,
              admin: this.admin,
              core: false
            }

          } 
        }
        

        let begin: Date = this.startDate;
        let end: Date = this.endDate;
        this.model = {
                        isRange: true, 
                        singleDate: null, 
                        dateRange: {
                          beginDate: {
                            year: begin.getFullYear(), month: begin.getMonth() + 1, day: begin.getDate()
                          },
                          endDate: {
                            year: end.getFullYear(), month: end.getMonth() + 1, day: end.getDate()
                          }
                        }
                      };




        this.refresh = new Subject();
    
        this.trainings$ = this.refresh.asObservable()
          .pipe(
            startWith('onInit'), // Emit value to force load on page load; actual value does not matter
            flatMap(_ => this.service.getCustom(this.criteria)), // Get some items
            tap(_ => this.loading = false) // Turn off the spinner
          );
        }
    );





    

  }

  dateCnanged(event: IMyDateModel){
    this.startDate = event.dateRange.beginJsDate;
    this.endDate = event.dateRange.endJsDate;
    this.criteria["start"] = this.startDate.toISOString();
    this.criteria["end"] = this.endDate.toISOString();
    this.onRefresh();
    //this.trainings$ = this.service.perPeriod(event.beginJsDate, event.endJsDate);
  }

  onSearch(event){
    this.criteria["search"] = event.target.value;
    this.onRefresh();
  }
  onSearchContact(event){
    this.criteria["contacts"] = event.target.value;
    this.onRefresh();
  }

  onRefresh() {
    this.loading = true; // Turn on the spinner.
    this.refresh.next('onRefresh'); // Emit value to force reload; actual value does not matter
  }
  deletedTraining(_: any){
    this.onRefresh();
  }
  switchOrder(type:string){
    this.type = type;
    this.criteria["order"] = type;
    this.onRefresh();
  }

  onSeatsChange(event){
    this.criteria["withseats"] = event.target.checked;
    this.onRefresh();
  }
  onIsCoreChange(event){
    this.criteria["core"] = event.target.checked;
    this.onRefresh();
  }

  onDayChange(event){
    this.criteria["day"] = event.target.value;
    this.onRefresh();
  }

}
