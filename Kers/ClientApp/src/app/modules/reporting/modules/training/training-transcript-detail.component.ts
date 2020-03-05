import { Component, OnInit, Input } from '@angular/core';
import { Training, TrainingSurveyResult } from './training';
import { User } from '../user/user.service';

@Component({
  selector: '[training-transcript-detail]',
  templateUrl: './training-transcript-detail.component.html',
  styles: []
})

export class TrainingTranscriptDetailComponent implements OnInit {

  @Input('training-transcript-detail') training:Training;
  @Input() user:User;

  attended = false;
  surveyTaken = false;

  default = true;


  constructor() { }

  ngOnInit() {
    this.isAttended();
    this.survey();
  }

  attendance(){

    var attended = "NO"
    if( this.attended){
        attended = "YES"; 
    }
    return attended;
  }

  survey(){
    this.surveyTaken = false;
    var surveyTaken = this.training.surveyResults.filter( e => e.userId == this.user.id);
    if(surveyTaken.length != 0) this.surveyTaken = true;
  }


  isAttended( ):void{
    var enrollment = this.training.enrollment.filter( e => e.attendieId == this.user.id);
    if( enrollment.length != 0){
      if( enrollment[0].attended == true ){
        this.attended = true;
        return;
      }
    }
    this.attended = false;
  }
  surveySubmitte(event:TrainingSurveyResult){
    this.training.surveyResults.push(event);
    this.default = true;
    this.surveyTaken = true;
  }


}
