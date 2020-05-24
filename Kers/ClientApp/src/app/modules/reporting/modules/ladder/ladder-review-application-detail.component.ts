import { Component, OnInit, Input } from '@angular/core';
import { LadderApplication, LadderStage, LadderApplicationStage } from './ladder';
import { Observable } from 'rxjs';
import { LadderService } from './ladder.service';

@Component({
  selector: 'ladder-review-application-detail',
  templateUrl: './ladder-review-application-detail.component.html',
  styles: [
    `
    .row{
      padding-top: 10px;
      padding-bottom: 5px;
      border-bottom: 1px solid #ccc
    }
    `
  ]
})
export class LadderReviewApplicationDetailComponent implements OnInit {
  @Input() application:LadderApplication;

  nextStage$:Observable<LadderStage>;
  previousStage$:Observable<LadderStage>;
  

  reviewPending = false;
  reviewed = false;

  rowDefault = true;
  rowReview = false;
  notes:string;
  constructor(
    private service:LadderService
  ) { }

  ngOnInit() {
    this.nextStage$ = this.service.nextStage(this.application.lastStageId);
    this.previousStage$ = this.service.previousStage(this.application.lastStageId);
  }
  review(){
    this.rowDefault = false;
    this.rowReview = true;
  }
  default(){
    this.rowDefault = true;
    this.rowReview = false;
  }
  submit(approve:boolean){
    this.reviewPending = true;
    let stage = <LadderApplicationStage>{};
    stage.note = this.notes;
    stage.ladderApplicationId = this.application.id;
    this.service.review(stage,approve).subscribe(
      res => {
        this.reviewPending = false;
        this.reviewed = true;
        console.log( res );
      }
    )

    
  }

}
