import { Component, OnInit } from '@angular/core';
import { TrainingService } from './training.service';
import { Observable } from 'rxjs';
import { TrainingConvertItemComponent } from './training-convert-item.component';
import { Training } from './training';

@Component({
  selector: 'training-convert',
  templateUrl: './training-convert.component.html',
  styles: []
})
export class TrainingConvertComponent implements OnInit {

  services$: Observable<Object[]>;

  items: TrainingConvertItemComponent[] = [];
  trainings: Training[] = [];
  convertAllClicked = false;
  convertedItemIndex=0;

  constructor(
    private service:TrainingService
  ) { 
    this.services$ = service.getServices();
  }

  ngOnInit() {
  }

  addItem(item:TrainingConvertItemComponent){
    this.items.push(item);
  }

  addTraining(training:Training){
    this.trainings.push(training);
    if(this.convertAllClicked && this.convertedItemIndex < this.items.length - 1){
      this.items[++this.convertedItemIndex].convert();
      if(this.convertedItemIndex > this.items.length - 1) this.convertAllClicked = false;
    }
  }
  convertAll(){
    this.convertAllClicked = true;
    this.items[0].convert()
  }

}
