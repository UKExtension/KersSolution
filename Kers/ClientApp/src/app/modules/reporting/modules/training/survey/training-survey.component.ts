import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Training, TrainingSurveyResult } from '../training';
import { UserService, User } from '../../user/user.service';
import { TrainingService } from '../training.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { switchMap, tap } from 'rxjs/operators';
import { ReportingService } from '../../../components/reporting/reporting.service';
import {outlineJson} from './outline';

@Component({
  selector: 'training-survey',
  templateUrl: './training-survey.component.html',
  styles: []
})
export class TrainingSurveyComponent implements OnInit {
  @Input() training:Training;
  @Input() user:User;
  @Input() redirect = true;
  @Output() submitSurvey = new EventEmitter<TrainingSurveyResult>();
  loading = true;
  isQualified = false;
  attended = false;
  completed = true;
  json:object;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service:TrainingService,
    private userService:UserService,
    private reportingService: ReportingService
  ) { }

  ngOnInit() {
    this.json = outlineJson;
    if( this.training == null){
      this.route.paramMap.pipe(
        switchMap((params: ParamMap) =>{
            return this.service.getTraining(+params.get('id'))
          }
        )
      ).subscribe(
        res =>{
          this.training = res;
          this.checkIfQualified();
        } 
      );
    }
    if(this.user == null){
      this.userService.current().subscribe(
        res => {
          this.user = res;
          this.checkIfQualified();
        }
      )
    }
    this.checkIfQualified();
  }


  checkIfQualified(){
    if(this.training != null && this.user != null){
      this.checkIfAttended();
      if(this.attended == true) this.checkIfCompleted();
     
      if(this.attended == true && this.completed == false){
        this.isQualified = true;
        this.loading = false;
        return;
      } 
      this.loading = false;
      
    }
    this.isQualified = false;

  }

  checkIfAttended(){
    if(this.training != null && this.user!=null){
      var enrollment = this.training.enrollment.filter( e => e.attendieId == this.user.id);
      if( enrollment.length != 0){
        
        if( enrollment[0].attended == true ){
          this.attended = true;
          return;
        }
      }
    }
    this.attended = false;
  }

  checkIfCompleted(){
    if(this.training != null && this.user!=null){
      var surveyTaken = this.training.surveyResults.filter( e => e.userId == this.user.id);
      if(surveyTaken.length == 0){
        this.completed = false;
        return;
      } 
    }
    this.completed = true;
  }

  sendData(result) {
    this.loading = true;
    var obj = <TrainingSurveyResult>{};
    obj.userId = this.user.id;
    obj.trainingId = this.training.id;
    obj.result = JSON.stringify(result);
    this.service.addSurvey(obj).subscribe(
      res =>{
        if(this.redirect){
          this.reportingService.setAlert("Evaluation Survey was Submitted. Thank You.");
          this.router.navigate(['/reporting/training/catalog']);
        } 
        this.submitSurvey.emit(res);
        this.loading = false;
      }
    )
    
  }
/* 
  json = {
    "title": "In Service Training Evaluation",
    "pages": [
     {
      "name": "page1",
      "elements": [
       {
        "type": "matrix",
        "name": "1. Content",
        "title": "The Content",
        "columns": [
         {
          "value": "1",
          "text": "Strongly Disagree"
         },
         {
          "value": "2",
          "text": "Disagree"
         },
         {
          "value": "3",
          "text": "Neutral"
         },
         {
          "value": "4",
          "text": "Agree"
         },
         {
          "value": "5",
          "text": "Strongly Agree"
         }
        ],
        "rows": [
         {
          "value": "1.1",
          "text": "Was relevant to my needs."
         },
         {
          "value": "1.2",
          "text": "Was well organized."
         },
         {
          "value": "1.3",
          "text": "Was adequately related to the topic."
         },
         {
          "value": "1.4",
          "text": "Was easy to understand."
         }
        ]
       },
       {
        "type": "matrix",
        "name": "2. Instructors",
        "title": "The Instructors",
        "columns": [
         {
          "value": "1",
          "text": "Strongly Disagree"
         },
         {
          "value": "2",
          "text": "Disagree"
         },
         {
          "value": "3",
          "text": "Neutral"
         },
         {
          "value": "4",
          "text": "Agree"
         },
         {
          "value": "5",
          "text": "Strongly Agree"
         }
        ],
        "rows": [
         {
          "value": "2.1",
          "text": "Were well-prepared."
         },
         {
          "value": "2.2",
          "text": "Used teaching methods appropriate for the content/audience."
         },
         {
          "value": "2.3",
          "text": "Was knowledgeable of the subject matter."
         },
         {
          "value": "2.4",
          "text": "Engaged the participants in learning."
         },
         {
          "value": "2.5",
          "text": "Related program content to practical situations."
         },
         {
          "value": "2.6",
          "text": "Answered questions clearly and accurately."
         }
        ]
       },
       {
        "type": "matrix",
        "name": "3. Outcomes",
        "title": "Outcomes:",
        "columns": [
         {
          "value": "1",
          "text": "Strongly Disagree"
         },
         {
          "value": "2",
          "text": "Disagree"
         },
         {
          "value": "3",
          "text": "Neutral"
         },
         {
          "value": "4",
          "text": "Agree"
         },
         {
          "value": "5",
          "text": "Strongly Agree"
         }
        ],
        "rows": [
         {
          "value": "3.1",
          "text": "I gained knowledge/skills about the topics presented."
         },
         {
          "value": "3.2",
          "text": "I will use what I learned in my county program."
         },
         {
          "value": "3.3",
          "text": "This information will help my program move to the next level."
         },
         {
          "value": "3.4",
          "text": "Based on the in-service, I am now able to teach this topic to others."
         }
        ]
       },
       {
        "type": "text",
        "name": "4. Teach",
        "title": "Based on the in-service, I am now able to teach this topic to others."
       },
       {
        "type": "text",
        "name": "5. Encouraged",
        "title": "Based on this in-service, what are two things that you are encouraged to do within the next six (6) months?  "
       },
       {
        "type": "text",
        "name": "6. Program",
        "title": "If you have a program related to this topic, what do you think will help take it to the next level (i.e., achieve higher level impact)?"
       },
       {
        "type": "text",
        "name": "7. Training Comments",
        "title": "Please provide any additional comments about this training."
       },
       {
        "type": "text",
        "name": "8. Instructors",
        "title": "Please provide any comments about the instructor or any additional instructors/presenters."
       }
      ]
     }
    ]
   }
   */

}
