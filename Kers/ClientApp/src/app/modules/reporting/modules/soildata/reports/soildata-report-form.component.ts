import { Component, OnInit, Input } from '@angular/core';
import { SoilReportBundle } from '../soildata.report';

@Component({
  selector: 'soildata-report-form',
  templateUrl: './soildata-report-form.component.html',
  styles: []
})
export class SoildataReportFormComponent implements OnInit {
  @Input() report: SoilReportBundle;
  
  constructor() { }

  ngOnInit() {
    console.log(this.report);
  }

}
