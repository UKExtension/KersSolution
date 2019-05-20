import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { TrainingService } from './training.service';
import { Training } from './training';

@Component({
  selector: 'training-convert-item',
  templateUrl: './training-convert-item.component.html',
  styles: []
})
export class TrainingConvertItemComponent implements OnInit {
  @Input('service') s:Object;

  @Output() onLoaded = new EventEmitter<TrainingConvertItemComponent>();
  @Output() onConverted = new EventEmitter<Training>();

  loading = false;

  training:Training;

  constructor(
    private service:TrainingService
  ) { }

  ngOnInit() {
    this.onLoaded.emit(this);
  }
  convert(){
    this.loading = true;
    this.service.migrate(+this.s["rID"]).subscribe(
      res => {
        this.training = res;
        this.loading = false;
        this.onConverted.emit(this.training);
      }
    );
  }

}
