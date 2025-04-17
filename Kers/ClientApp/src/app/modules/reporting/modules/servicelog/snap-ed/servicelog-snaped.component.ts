import { Component, OnInit } from '@angular/core';
import { ServicelogMonth, Servicelog } from "../servicelog.service";
import { ReportingService } from "../../../components/reporting/reporting.service";
import { Router } from "@angular/router";
import { SnapedService } from "../snaped.service";



@Component({
  templateUrl: 'servicelog-snaped.component.html'
})
export class ServicelogSnapedComponent implements OnInit { 
    
    
    newActivity = false;
    latest = [];
    numbActivities = 0;
    byMonth:ServicelogMonth[] = [];


    newOff = true;
    newDirect = false;
    newIndirect = false;
    newPolicy = false;
    newAdmin = false;

    showCommitment = false;

    hours = 0;
    copies = 0;
    committed = 0;

    errorMessage:string;

    constructor( 
        private service:SnapedService,
        private reportingService: ReportingService,
        private router: Router
    )   
    {
        
    }

    direct(){
        this.newOff = false;
        this.newDirect = true;
    }
    indirect(){
        this.newOff = false;
        this.newIndirect = true;
    }
    policy(){
        this.newOff = false;
        this.newPolicy = true;
    }   
    admin(){
        this.newOff = false;
        this.newAdmin = true;
    }   
    closeNew(){
        this.newOff = true;
        this.newDirect = false;
        this.newIndirect = false;
        this.newAdmin = false;
        this.newPolicy = false;
    }

    ngOnInit(){


        this.service.reportedhours().subscribe(
            res => {

                this.hours = <number> res;
                
                this.service.reach().subscribe(
                    res => {
                        this.copies = <number>res;
                        this.service.committedhours().subscribe(
                            res => {
                                this.committed = <number>res;
                                this.addStats();
                            },
                            err => this.errorMessage = <any> err
                        )
                    },
                    err => this.errorMessage = <any>err
                )



               


            },
            err => this.errorMessage  = <any>err
        );

        this.reportingService.setTitle('Supplemental Nutrition Assistance Program');

        
        this.service.latest().subscribe(
            res=>{
                    this.latest = <Servicelog[]>res;
                    this.populateByMonth();
                },
            err => this.errorMessage = <any>err
        );
        this.service.num().subscribe(
            res => {
                this.numbActivities = res;
            },
            err => this.errorMessage = <any> err
        );

       
     
    }

    ngOnDestroy(){
        this.reportingService.hideStats();
        this.reportingService.setDefaultTitle();
    }

    addStats(){
        let percent = 0;
        if(this.committed == 0){
            percent = 100;
        }else{
            if(this.committed > 0){
                percent = Math.round(this.hours/this.committed  * 100);
             }
        }
        
        
        this.reportingService.addStats(`
        
        <div class="row top_tiles">
        <div class="animated flipInY col-lg-3 col-md-3 col-sm-6 col-xs-12">
          <div class="tile-stats">
            <div class="icon"><i class="fa fa-check-square-o"></i></div>
            <div class="count">`+ this.hours +`</div>
            <h3>Reported</h3>
            <p>Snap-Ed Hours.</p>
          </div>
        </div>
        <div class="animated flipInY col-lg-3 col-md-3 col-sm-6 col-xs-12">
          <div class="tile-stats">
            <div class="icon"><i class="fa fa-comments-o"></i></div>
            <div class="count">`+ this.committed +`</div>
            <h3>Commited</h3>
            <p>Snap-Ed Hours.</p>
          </div>
        </div>
        <div class="animated flipInY col-lg-3 col-md-3 col-sm-6 col-xs-12">
          <div class="tile-stats">
            <div class="icon"><i class="fa fa-bookmark-o"></i></div>
            <div class="count">` + percent + `</div>
            <h3>Percent</h3>
            <p>Commitment Fulfilled..</p>
          </div>
        </div>
        <div class="animated flipInY col-lg-3 col-md-3 col-sm-6 col-xs-12">
          <div class="tile-stats">
            <div class="icon"><i class="fa fa-arrows-h"></i></div>
            <div class="count">` + this.copies + `</div>
            <h3>Direct Ed</h3>
            <p>Reach.</p>
          </div>
        </div>
      </div>
        
        `);
    }


    populateByMonth(){
        var bm = this.byMonth;
        this.latest.forEach(function(element){
            
                var exDt = new Date(element.activityDate);
                var exMonth = bm.filter(f=>f.month==exDt.getMonth() && f.year == exDt.getFullYear());
                if(exMonth.length == 0){
                    var ob = <ServicelogMonth>{
                        month : exDt.getMonth(),
                        year : exDt.getFullYear(),
                        date: exDt,
                        activities : [element]
                    };
                    bm.push(ob);
                }else{
                    exMonth[0].activities.push(element);
                }
            }); 
    }



    edited(activity:Servicelog){
        
        this.latest = this.latest.map(function(item) { return item.activityId == activity.activityId ? activity : item; });
        this.latest.sort(
                    function(a, b) {

                        var dateA = new Date(a.activityDate);
                        var dateB = new Date(b.activityDate);
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
    deleted(activity:Servicelog){
        let index: number = this.latest.indexOf(activity);
        if (index !== -1) {
            this.latest.splice(index, 1);
            this.numbActivities--;
        }
        this.byMonth = [];
        this.populateByMonth();
    }

    loadMore(){
        var lt = this.latest;
        this.service.latest(this.latest.length, 5).subscribe(
            res=>{
                    var batch = <Servicelog[]>res; 
                    batch.forEach(function(element){
                        lt.push(element);
                    });
                    this.byMonth = [];
                    this.populateByMonth();
                },
            err => this.errorMessage = <any>err
        );
    }

    newActivitySubmitted(servicelog:Servicelog){
        if(servicelog.isSnap){
            this.latest.unshift(servicelog);
            this.byMonth = [];
            this.populateByMonth();
            this.newActivity = false;
            this.numbActivities++;
        }
        this.closeNew();
    }


}