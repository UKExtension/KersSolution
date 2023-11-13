import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TaxExempt } from './exmpt';

@Component({
  selector: '[exempt-list-detail]',
  templateUrl: './exempt-list-detail.component.html',
  styles: [
  ]
})
export class ExemptListDetailComponent implements OnInit {
  
  editOppened = false;
  defaultView = true;
  deleteOppened = false;

  @Input ('exempt-list-detail') exempt: TaxExempt;

  @Output() onExemptUpdated = new EventEmitter();
  @Output() onExemptDeleted = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  edit(){
    this.defaultView = false;
    this.deleteOppened = false;
    this.editOppened = true;
  }
  pdf(){

  }
  default(){
      this.defaultView = true;
      this.deleteOppened = false;
      this.editOppened = false;
  }
  delete(){
      this.defaultView = false;
      this.deleteOppened = true;
      this.editOppened = false;
  }

  edited(newExempt:TaxExempt){
    this.exempt = newExempt;
    this.default();
  }

  programAreas():string{
    var areas = "";
    var ars = this.exempt.taxExemptProgramCategories.map( c => c.taxExemptProgramCategory.name);
    areas = ars.join(', ');
    return areas;
  }

}
