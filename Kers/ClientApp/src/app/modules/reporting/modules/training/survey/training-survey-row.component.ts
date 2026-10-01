import { Component, OnInit, Input, ChangeDetectionStrategy } from '@angular/core';
import { TrainingSurveyResult } from '../training';
import {outlineJson} from './outline';

@Component({
    selector: '[training-survey-row]',
    template: `
    @if (default) {
      <td>{{result.created | date:'mediumDate'}}</td>
    }
    @if (default) {
      <td><a (click)="detailsView()" class="btn btn-info btn-xs pull-right">details</a></td>
    }
    @if (details) {
      <td colspan="2"><br>
        <div>
          <a class="btn btn-info btn-xs pull-right" (click)="defaultView()">close survey details</a>
        </div>
        <br><br>
        <div>
          <survey [json]="json" [previousResult]="previousResult"></survey>
        </div>
      </td>
    }
    
    `,
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class TrainingSurveyRowComponent implements OnInit {
  @Input('training-survey-row') result:TrainingSurveyResult;
  default = true;
  details = false;
  constructor() { }
  json:object;
  previousResult:object;

  ngOnInit() {
    this.json = outlineJson;
    this.previousResult = JSON.parse(this.result.result);
  }
  defaultView(){
    this.default = true;
    this.details = false;
  }
  detailsView(){
    this.details = true;
    this.default = false;
  }

}
