import { Component, Input, forwardRef, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
    selector: 'timepicker',
    template: `
<select class="form-control col-md-4 col-xs-7" (change)="changeTime($event)" [value]="timeValue">
    <option value="">{{empty}}</option>
    <option value="00:00">12:00 AM (00:00)</option>
    <option value="00:30">12:30 AM (00:30)</option>
    <option value="01:00">1:00 AM (01:00)</option>
    <option value="01:30">1:30 AM (01:30)</option>
    <option value="02:00">2:00 AM (02:00)</option>
    <option value="02:30">2:30 AM (02:30)</option>
    <option value="03:00">3:00 AM (03:00)</option>
    <option value="03:30">3:30 AM (03:30)</option>
    <option value="04:00">4:00 AM (04:00)</option>
    <option value="04:30">4:30 AM (04:30)</option>
    <option value="05:00">5:00 AM (05:00)</option>
    <option value="05:30">5:30 AM (05:30)</option>
    <option value="06:00">6:00 AM (06:00)</option>
    <option value="06:30">6:30 AM (06:30)</option>
    <option value="07:00">7:00 AM (07:00)</option>
    <option value="07:30">7:30 AM (07:30)</option>
    <option value="08:00">8:00 AM (08:00)</option>
    <option value="08:30">8:30 AM (08:30)</option>
    <option value="09:00">9:00 AM (09:00)</option>
    <option value="09:30">9:30 AM (09:30)</option>
    <option value="10:00">10:00 AM (10:00)</option>
    <option value="10:30">10:30 AM (10:30)</option>
    <option value="11:00">11:00 AM (11:00)</option>
    <option value="11:30">11:30 AM (11:30)</option>
    <option value="12:00">12:00 PM (12:00)</option>
    <option value="12:30">12:30 PM (12:30)</option>
    <option value="13:00">1:00 PM (13:00)</option>
    <option value="13:30">1:30 PM (13:30)</option>
    <option value="14:00">2:00 PM (14:00)</option>
    <option value="14:30">2:30 PM (14:30)</option>
    <option value="15:00">3:00 PM (15:00)</option>
    <option value="15:30">3:30 PM (15:30)</option>
    <option value="16:00">4:00 PM (16:00)</option>
    <option value="16:30">4:30 PM (16:30)</option>
    <option value="17:00">5:00 PM (17:00)</option>
    <option value="17:30">5:30 PM (17:30)</option>
    <option value="18:00">6:00 PM (18:00)</option>
    <option value="18:30">6:30 PM (18:30)</option>
    <option value="19:00">7:00 PM (19:00)</option>
    <option value="19:30">7:30 PM (19:30)</option>
    <option value="20:00">8:00 PM (20:00)</option>
    <option value="20:30">8:30 PM (20:30)</option>
    <option value="21:00">9:00 PM (21:00)</option>
    <option value="21:30">9:30 PM (21:30)</option>
    <option value="22:00">10:00 PM (22:00)</option>
    <option value="22:30">10:30 PM (22:30)</option>
    <option value="23:00">11:00 PM (23:00)</option>
    <option value="23:30">11:30 PM (23:30)</option>
</select>
    `,
    providers:[  { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => TiimepickerComponent),
                    multi: true
                  } 
                ]
})

export class TiimepickerComponent{

    @Input('empty') empty = "-- select time --";


    public propagateChange:any;

    _value = 0;


    get timeValue() {
      return this._value;
    }
    set timeValue(val) {
        this._value = val;     
        this.propagateChange(val);
    }


    constructor(){
        this.propagateChange = () => {};
    }


    writeValue(value: any) {
        this.timeValue = value;
    }


    registerOnChange(fn) {
        this.propagateChange = fn;
    }

    registerOnTouched() {}    




    changeTime(selection){
        this.writeValue(selection.target.value);
    }

}