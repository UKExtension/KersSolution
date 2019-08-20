import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';



@Component({
  selector: 'budget-user-defined-income',
  template: `
<div class="form-group">
  <div class="form-inline">
      <div class="form-group">
          <input type="text" class="form-control" /><br>
          <small>(Source of Income)</small>
      </div>
      &nbsp;
      <div class="form-group">
          <input type="number" class="form-control col-md-3 col-xs-6"/><br>
          <small>(Amount)</small>
      </div>
      <div class="form-group">
              <div class="col-xs-1 ng-star-inserted"><span><a class="close-link" (click)="onRemove()" style="cursor:pointer;"><i class="fa fa-close"></i></a></span></div>
              <br>
          <small>&nbsp;</small>
      </div>


  </div>
</div>

  `,
  providers:[  { 
                  provide: NG_VALUE_ACCESSOR,
                  useExisting: forwardRef(() => BudgetUserDefinedIncomeComponent),
                  multi: true
                } 
                ]
})
export class BudgetUserDefinedIncomeComponent implements OnInit { 
/* 
    @Input('group') public connectionForm:FormGroup;
    @Input('canDelete') canDelete:boolean;
    
    @Input('connectionTypes') connectionTypes:SocialConnectionType[];

    

    public selectedLabel = "Select Social Media ";
 */
    @Input('index') index:number;
    @Input() _value = 0;



    @Output() removeMe = new EventEmitter<number>();


    
    propagateChange:any = () => {};



    constructor( 
        
    )   
    {}


/*     selectedConnection(type){
        this.selectedLabel = type.name;
    }
*/
    onRemove(){
        this.removeMe.emit(this.index);
    } 

    ngOnInit(){
        
        
    }
    writeValue(value: any) {
      if (value !== "") {
        
      }
  }


  registerOnChange(fn) {
    this.propagateChange = fn;
  }

  registerOnTouched() {}

}