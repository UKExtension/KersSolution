import { Component, Input, forwardRef, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'social-picker',
    template: `
            <button type="button" (click)="menuOpen = !menuOpen" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-expanded="false">{{selectedLabel}} <span class="caret"></span>
            </button>
            @if (connectionTypes) {
              <ul class="dropdown-menu dropdown-menu-right" role="menu" [ngStyle]="{'display': menuOpen ? 'block':'none'}">
                @for (type of connectionTypes; track type) {
                  <li (click)="selectedConnection(type)"><a ><span class="fa {{type.icon}}" aria-hidden="true"></span> {{type.name}}</a></li>
                }
              </ul>
            }
            `,
    providers: [{
            provide: NG_VALUE_ACCESSOR,
            useExisting: forwardRef(() => UserSocialPickerComponent),
            multi: true
        }
    ],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class UserSocialPickerComponent implements ControlValueAccessor, OnInit {

  @Input('types') connectionTypes;

  @Input() _value = 0;

  propagateChange:any = () => {};

  selectedLabel = "Select Social Media ";

  menuOpen = false;

  ngOnInit(){
      this.selectedLabel = "Select Social Media ";
  }

  selectedConnection(type){
        this.selectedLabel = type.name;
        this._value = type.id;
        this.propagateChange(type.id);
        this.menuOpen = false;
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