import { Component, OnInit, Input } from '@angular/core';
import { Training } from '../training';
import { UserService, User } from '../../user/user.service';
import { TrainingService } from '../training.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap, tap } from 'rxjs/operators';

@Component({
  selector: 'training-survey',
  templateUrl: './training-survey.component.html',
  styles: []
})
export class TrainingSurveyComponent implements OnInit {
  @Input() training:Training;
  @Input() user:User;
  loading = true;
  isQualified = false;
  attended = false;
  completed = true;

  constructor(
    private route: ActivatedRoute,
    private service:TrainingService,
    private userService:UserService
  ) { }

  ngOnInit() {
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
    //TODO update with your own behavior    
    console.log(result);
  }

  json = {
    "title": "In Service Training Evaluation",
    "pages": [
     {
      "name": "page1",
      "elements": [
       {
        "type": "matrix",
        "name": "The Content",
        "columns": [
         {
          "value": "Column 1",
          "text": "Strongly Disagree"
         },
         {
          "value": "Column 2",
          "text": "Disagree"
         },
         {
          "value": "Column 3",
          "text": "Neutral"
         },
         {
          "value": "Column 4",
          "text": "Agree"
         },
         {
          "value": "Column 5",
          "text": "Strongly Agree"
         }
        ],
        "rows": [
         {
          "value": "Row 1",
          "text": "Was relevant to my needs."
         },
         {
          "value": "Row 2",
          "text": "Was well organized."
         },
         {
          "value": "Row 3",
          "text": "Was adequately related to the topic."
         },
         {
          "value": "Row 4",
          "text": "Was easy to understand."
         }
        ]
       },
       {
        "type": "matrix",
        "name": "question2",
        "title": "The Instructors",
        "columns": [
         {
          "value": "Column 1",
          "text": "Strongly Disagree"
         },
         {
          "value": "Column 2",
          "text": "Disagree"
         },
         {
          "value": "Column 3",
          "text": "Neutral"
         },
         {
          "value": "Column 4",
          "text": "Agree"
         },
         {
          "value": "Column 5",
          "text": "Strongly Agree"
         }
        ],
        "rows": [
         {
          "value": "Row 1",
          "text": "Were well-prepared."
         },
         {
          "value": "Row 2",
          "text": "Used teaching methods appropriate for the content/audience."
         },
         {
          "value": "Row 3",
          "text": "Was knowledgeable of the subject matter."
         },
         {
          "value": "Row 4",
          "text": "Engaged the participants in learning."
         },
         {
          "value": "Row 5",
          "text": "Related program content to practical situations."
         },
         {
          "value": "Row 6",
          "text": "Answered questions clearly and accurately."
         }
        ]
       },
       {
        "type": "matrix",
        "name": "question1",
        "title": "Outcomes:",
        "columns": [
         {
          "value": "Column 1",
          "text": "Strongly Disagree"
         },
         {
          "value": "Column 2",
          "text": "Disagree"
         },
         {
          "value": "Column 3",
          "text": "Neutral"
         },
         {
          "value": "Column 4",
          "text": "Agree"
         },
         {
          "value": "Column 5",
          "text": "Strongly Agree"
         }
        ],
        "rows": [
         {
          "value": "Row 1",
          "text": "I gained knowledge/skills about the topics presented."
         },
         {
          "value": "Row 2",
          "text": "I will use what I learned in my county program."
         },
         {
          "value": "Row 3",
          "text": "This information will help my program move to the next level."
         },
         {
          "value": "Row 4",
          "text": "Based on the in-service, I am now able to teach this topic to others."
         }
        ]
       },
       {
        "type": "text",
        "name": "question3",
        "title": "Based on the in-service, I am now able to teach this topic to others."
       },
       {
        "type": "text",
        "name": "question4",
        "title": "Based on this in-service, what are two things that you are encouraged to do within the next six (6) months?  "
       },
       {
        "type": "text",
        "name": "question5",
        "title": "If you have a program related to this topic, what do you think will help take it to the next level (i.e., achieve higher level impact)?"
       },
       {
        "type": "text",
        "name": "question6",
        "title": "Please provide any additional comments about this training."
       },
       {
        "type": "text",
        "name": "question7",
        "title": "Please provide any comments about the instructor or any additional instructors/presenters."
       }
      ],
      "title": "The Content:"
     }
    ]
   }
  

}
