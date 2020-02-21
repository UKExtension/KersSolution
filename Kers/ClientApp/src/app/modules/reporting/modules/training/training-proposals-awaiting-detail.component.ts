import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Training } from './training';
import { TrainingService } from './training.service';

@Component({
  selector: '[training-proposals-awaiting-detail]',
  templateUrl: './training-proposals-awaiting-detail.component.html',
  styles: []
})
export class TrainingProposalsAwaitingDetailComponent implements OnInit {

  @Input('training-proposals-awaiting-detail') training: Training;
  @Output() onEdit = new EventEmitter<Training>();
  @Output() onDeleted = new EventEmitter<Training>();

  default = true;
  edit = false;
  delete = false;
  errorMessage: string;


  constructor(
    private service:TrainingService
  ) { }

  ngOnInit() {
  }
  defaultView(){
    this.default = true;
    this.edit = false;
    this.delete = false;
  }
  editView(){
    this.default = false;
    this.edit = true;
    this.delete = false;
  }
  deleteView(){
    this.default = false;
    this.edit = false;
    this.delete = true;
  }
  editSubmitted(training:Training){
    this.training = training;
    this.defaultView();
    this.onEdit.emit(training);
  }
  confirmDelete(){
        
    this.service.delete(this.training.id).subscribe(
        res=>{
            this.defaultView();
            this.onDeleted.emit(this.training);  
        },
        err => this.errorMessage = <any> err
    );
    
}

}
