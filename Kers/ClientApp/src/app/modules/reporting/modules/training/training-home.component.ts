import { Component, OnInit } from '@angular/core';
import {TrainingService} from './training.service';
import { Training } from './training';
import { Observable } from 'rxjs';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  selector: 'training-home',
  templateUrl: './training-home.component.html',
  styleUrls: ['./training-home.component.css']
})
export class TrainingHomeComponent implements OnInit {


  constructor(
    private reportingService: ReportingService
  ) { }

  ngOnInit() {
    this.defaultTitle();
  }
  defaultTitle(){
    this.reportingService.setTitle("In-Service Training");
    this.reportingService.setSubtitle("For specific In-Service related questions or assistance, please email: agpsd@lsv.uky.edu");
  }
  ngOnDestroy(){
    this.reportingService.setTitle("Kentucky Extension Reporting System");
    this.reportingService.setSubtitle("");
  }
}