import { Component, OnInit, Input } from '@angular/core';
import { TrainingSurveyResult } from '../training';
import {outlineJson} from './outline';

@Component({
  selector: '[training-survey-row]',
  template: `
    <td *ngIf="default">{{result.created | date:'mediumDate'}}</td>
    <td *ngIf="default"><a (click)="detailsView()" class="btn btn-info btn-xs">details</a></td>
    <td *ngIf="details" colspan="2"><br>
      <div>
        <a class="btn btn-info btn-xs pull-right" (click)="defaultView()">close survey details</a>
      </div>
      <br><br>
      <div>
        <survey [json]="json" [previousResult]="previousResult"></survey>
      </div>
      
    </td>

  `,
  styles: []
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
