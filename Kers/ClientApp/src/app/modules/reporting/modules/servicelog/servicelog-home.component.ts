import { Component, OnInit } from '@angular/core';
import { ServicelogService, ServicelogMonth, Servicelog } from "./servicelog.service";
import { FiscalYear } from '../admin/fiscalyear/fiscalyear.service';
import { ActivatedRoute } from '@angular/router';


@Component({
  templateUrl: 'servicelog-home.component.html'
})
export class ServicelogHomeComponent implements OnInit { 
    
    
    newActivity = false;
    latest = [];
    numbActivities = 0;
    byMonth:ServicelogMonth[] = [];

    errorMessage:string;
    fiscalyearid:string | null = null;

    constructor( 
        private route: ActivatedRoute,
        private service:ServicelogService
    )   
    {}

    ngOnInit(){
        this.fiscalyearid = this.route.snapshot.paramMap.get('fy');
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

    newActivitySubmitted(servicelog){
        this.latest.unshift(servicelog);
        this.byMonth = [];
        this.populateByMonth();
        this.newActivity = false;
        this.numbActivities++;
        window.scrollTo(0,0);
    }


}