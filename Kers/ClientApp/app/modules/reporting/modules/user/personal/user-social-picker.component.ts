import { Component, Input, forwardRef, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'social-picker',
  template: `
            <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-expanded="false">{{selectedLabel}} <span class="caret"></span>
            </button>
            <ul class="dropdown-menu dropdown-menu-right" role="menu" *ngIf="connectionTypes">
                <li *ngFor="let type of connectionTypes" (click)="selectedConnection(type)"><a ><span class="fa {{type.icon}}" aria-hidden="true"></span> {{type.name}}</a></li>
            </ul>
  `,
    providers:[  { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => UserSocialPickerComponent),
                    multi: true
                  } 
                  ]
})
export class UserSocialPickerComponent implements ControlValueAccessor, OnInit {

  @Input('types') connectionTypes;

  @Input() _value = 0;

  propagateChange:any = () => {};

  selectedLabel = "Select Social Media ";

  ngOnInit(){
      this.selectedLabel = "Select Social Media ";
  }

  selectedConnection(type){
        this.selectedLabel = type.name;
        this._value = type.id;
        this.propagateChange(type.id);
  }

  writeValue(value: any) {
      if (value !== "") {
        if(this.connectionTypes != null){
          var tp = this.connectionTypes.filter(l=>l.id == <number>value)[0];
          if(tp != null){
              this.selectedLabel = tp.name;
              this._value = tp.id;
          } 
        }
      }
  }


  registerOnChange(fn) {
    this.propagateChange = fn;
  }

  registerOnTouched() {}

  
}