import { Component, OnInit } from '@angular/core';
import {TrainingService} from './training.service';
import { Training } from './training';
import { Observable } from 'rxjs';

@Component({
  selector: 'training-home',
  templateUrl: './training-home.component.html',
  styleUrls: ['./training-home.component.css']
})
export class TrainingHomeComponent implements OnInit {

  trainings:Observable<Training[]>;

  constructor(
    private service:TrainingService
  ) { }

  ngOnInit() {
    this.trainings = this.service.range();
  }

}
