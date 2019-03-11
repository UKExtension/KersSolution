import { Component, OnInit } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';
import { Observable } from 'rxjs';
import { Training } from './training';
import { TrainingService } from './training.service';

@Component({
  selector: 'app-training-catalog',
  templateUrl: './training-catalog.component.html',
  styles: []
})
export class TrainingCatalogComponent implements OnInit {
  colorTheme = 'theme-dark-blue';
  bsConfig: Partial<BsDatepickerConfig>;
  trainings:Observable<Training[]>;

  constructor(
    private service:TrainingService
  ) { 
    this.trainings = service.range();
  }

  ngOnInit() {
    this.bsConfig = Object.assign({}, { containerClass: this.colorTheme });
  }

}
