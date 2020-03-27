import { Component, OnInit, Input } from '@angular/core';
import { Activity } from '../../activity.service';

@Component({
  selector: '[service-log-summary-row]',
  templateUrl: './service-log-summary-row.component.html'
})
export class ServiceLogSummaryRowComponent implements OnInit {
  @Input('service-log-summary-row') activity:Activity;
  detailsOpened = false;
  constructor() { }

  ngOnInit() {
  }

  attendance(activity: Activity){
    var sum = 0;
    for(var r of activity.raceEthnicityValues){
        sum += r.amount;
    }
    return sum;
  }

  open(){
    this.detailsOpened = true;
  }
  close(){
    this.detailsOpened = false;
  }
  replaceImageTag(s:string):string{
    var re = /<img/gi; 
    var str = "Apples are round, and apples are juicy.";
    var newstr = s.replace(re, "<img width=50"); 
    return newstr;
  }

}
