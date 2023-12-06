import { Component, Input, OnInit } from '@angular/core';
import { TaxExempt } from './exmpt';
import { ExemptService } from './exempt.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'exempt-list',
  templateUrl: './exempt-list.component.html',
  styles: [
  ]
})
export class ExemptListComponent implements OnInit {
  exempts$:Observable<TaxExempt[]>;
  newExempt = false;
  constructor(
    private service:ExemptService
  ) {
    
   }

  ngOnInit(): void {
    this.exempts$=this.service.exemptsList();
  }

  onExemptUpdate(){
    this.exempts$=this.service.exemptsList();
    this.newExempt=false;
  }

}
