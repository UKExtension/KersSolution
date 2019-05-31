import { Component, OnInit } from '@angular/core';
import { TrainingService } from './training.service';
import { Observable } from 'rxjs';
import { Training } from './training';

@Component({
  selector: 'training-proposals-awaiting',
  templateUrl: './training-proposals-awaiting.component.html',
  styles: []
})
export class TrainingProposalsAwaitingComponent implements OnInit {
  trainings:Observable<Training[]>;
  constructor(
    private service:TrainingService
  ) {
    this.trainings = service.proposals();
   }

  ngOnInit() {
  }
  trainingEdited(_){
    this.trainings = this.service.proposals();
  }

}
