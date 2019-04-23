import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { Observable } from 'rxjs';
import { Training } from './training';
import { TrainingService } from './training.service';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'training-info',
  templateUrl: './training-info.component.html',
  styleUrls: ['./training-info.component.css']
})
export class TrainingInfoComponent implements OnInit {
  
  training$: Observable<Training>;

  constructor(
    private route: ActivatedRoute,
    private service: TrainingService,
  ) { }

  ngOnInit() {
    this.training$ = this.route.paramMap.pipe(
      switchMap((params: ParamMap) =>
        this.service.getTraining(+params.get('id')))
    );
  }

  registerCutOfDate(training:Training):Date{
    var start = new Date(training.start);
    if( training.registerCutoffDays == null) return start;
    start.setDate( start.getDate() - +training.registerCutoffDays.registerDaysVal );
    return start;
  }

  cancelCutOfDate(training:Training):Date{
    var start = new Date(training.start);
    if( training.cancelCutoffDays == null) return start;
    start.setDate( start.getDate() - training.cancelCutoffDays.cancelDaysVal );
    return start;
  }

}
