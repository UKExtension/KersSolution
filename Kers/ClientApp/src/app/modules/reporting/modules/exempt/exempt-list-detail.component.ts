import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TaxExempt } from './exmpt';
import { saveAs } from 'file-saver';
import { ExemptService } from './exempt.service';

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
  loading = false;

  @Input ('exempt-list-detail') exempt: TaxExempt;
  @Input () canEdit: boolean = true;

  @Output() onExemptUpdated = new EventEmitter();
  @Output() onExemptDeleted = new EventEmitter();

  constructor(
    private service:ExemptService
  ) { }

  ngOnInit(): void {
  }

  edit(){
    this.defaultView = false;
    this.deleteOppened = false;
    this.editOppened = true;
  }
  pdf(){

    this.loading = true;
    this.service.pdf(this.exempt.id).subscribe(
        data => {
            var blob = new Blob([data], {type: 'application/pdf'});
            saveAs(blob, "TaxExemptEntity_#"+this.exempt.id+".pdf");
            this.loading = false;
        },
        err => console.error(err)
    )


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
  confirmDelete(){
    this.loading = true;
    this.service.delete(this.exempt.id).subscribe(
        res=>{
            this.onExemptDeleted.emit();
            this.loading = false;
        }
    );
    
  }

}
