import { Component, Input, forwardRef, OnInit, Injector } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, NgControl, NG_VALIDATORS, AbstractControl, ValidationErrors } from '@angular/forms';

@Component({
    selector: 'timepicker',
    template: `
<select class="form-control col-md-4 col-xs-7" (change)="changeTime($event)" [value]="timeValue" [ngClass]="control?.valid ? 'ng-valid' : 'ng-invalid'" *ngIf="options != null">
    <option value="">{{empty}}</option>
    <option *ngFor="let tm of options" [value]="tm.value">{{tm.label}}</option>
</select>
    `,
    providers:[  { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => TiimepickerComponent),
                    multi: true
                  }
                ]
})

export class TiimepickerComponent implements OnInit{

    @Input('empty') empty = "-- select time --";
    @Input() start:number | null;
    @Input() end:number | null;
    options:TimeChoice[];

    allOptions:TimeChoice[] = [
        <TimeChoice>{
            id: 1,
            value: "00:00",
            label: "12:00 AM (00:00)"
        },
        <TimeChoice>{
            id: 2,
            value: "00:30",
            label: "12:30 AM (00:30)"
        },
        <TimeChoice>{
            id: 3,
            value: "01:00",
            label: "1:00 AM (01:00)"
        },
        <TimeChoice>{
            id: 4,
            value: "01:30",
            label: "1:30 AM (01:30)"
        },
        <TimeChoice>{
            id: 5,
            value: "02:00",
            label: "2:00 AM (02:00)"
        },
        <TimeChoice>{
            id: 6,
            value: "02:30",
            label: "2:30 AM (02:30)"
        },
        <TimeChoice>{
            id: 7,
            value: "03:00",
            label: "3:00 AM (03:00)"
        },
        <TimeChoice>{
            id: 8,
            value: "03:30",
            label: "3:30 AM (03:30)"
        },
        <TimeChoice>{
            id: 9,
            value: "04:00",
            label: "4:00 AM (04:00)"
        },
        <TimeChoice>{
            id: 10,
            value: "04:30",
            label: "4:30 AM (04:30)"
        },
        <TimeChoice>{
            id: 11,
            value: "05:00",
            label: "5:00 AM (05:00)"
        },
        <TimeChoice>{
            id: 12,
            value: "05:30",
            label: "5:30 AM (05:30)"
        },
        <TimeChoice>{
            id: 13,
            value: "06:00",
            label: "6:00 AM (06:00)"
        },
        <TimeChoice>{
            id: 14,
            value: "06:30",
            label: "6:30 AM (06:30)"
        },
        <TimeChoice>{
            id: 15,
            value: "07:00",
            label: "7:00 AM (07:00)"
        },
        <TimeChoice>{
            id: 16,
            value: "07:30",
            label: "7:30 AM (07:30)"
        },
        <TimeChoice>{
            id: 17,
            value: "08:00",
            label: "8:00 AM (08:00)"
        },
        <TimeChoice>{
            id: 18,
            value: "08:30",
            label: "8:30 AM (08:30)"
        },
        <TimeChoice>{
            id: 19,
            value: "09:00",
            label: "9:00 AM (09:00)"
        },
        <TimeChoice>{
            id: 20,
            value: "09:30",
            label: "9:30 AM (09:30)"
        },
        <TimeChoice>{
            id: 21,
            value: "10:00",
            label: "10:00 AM (10:00)"
        },
        <TimeChoice>{
            id: 22,
            value: "10:30",
            label: "10:30 AM (10:30)"
        },
        <TimeChoice>{
            id: 23,
            value: "11:00",
            label: "11:00 AM (11:00)"
        },
        <TimeChoice>{
            id: 24,
            value: "11:30",
            label: "11:30 AM (11:30)"
        },
        <TimeChoice>{
            id: 25,
            value: "12:00",
            label: "12:00 PM (12:00)"
        },
        <TimeChoice>{
            id: 26,
            value: "12:30",
            label: "12:30 PM (12:30)"
        },
        <TimeChoice>{
            id: 27,
            value: "13:00",
            label: "1:00 PM (13:00)"
        },
        <TimeChoice>{
            id: 28,
            value: "13:30",
            label: "1:30 PM (13:30)"
        },
        <TimeChoice>{
            id: 29,
            value: "14:00",
            label: "2:00 PM (14:00)"
        },
        <TimeChoice>{
            id: 30,
            value: "14:30",
            label: "2:30 PM (14:30)"
        },
        <TimeChoice>{
            id: 31,
            value: "15:00",
            label: "3:00 PM (15:00)"
        },
        <TimeChoice>{
            id: 32,
            value: "15:30",
            label: "3:30 PM (15:30)"
        },
        <TimeChoice>{
            id: 33,
            value: "16:00",
            label: "4:00 PM (16:00)"
        },
        <TimeChoice>{
            id: 34,
            value: "16:30",
            label: "4:30 PM (16:30)"
        },
        <TimeChoice>{
            id: 35,
            value: "17:00",
            label: "5:00 PM (17:00)"
        },
        <TimeChoice>{
            id: 36,
            value: "17:30",
            label: "5:30 PM (17:30)"
        },
        <TimeChoice>{
            id: 37,
            value: "18:00",
            label: "6:00 PM (18:00)"
        },
        <TimeChoice>{
            id: 38,
            value: "18:30",
            label: "6:30 PM (18:30)"
        },
        <TimeChoice>{
            id: 39,
            value: "19:00",
            label: "7:00 PM (19:00)"
        },
        <TimeChoice>{
            id: 40,
            value: "19:30",
            label: "7:30 PM (19:30)"
        },
        <TimeChoice>{
            id: 41,
            value: "20:00",
            label: "8:00 PM (20:00)"
        },
        <TimeChoice>{
            id: 42,
            value: "20:30",
            label: "8:30 PM (20:30)"
        },
        <TimeChoice>{
            id: 43,
            value: "21:00",
            label: "9:00 PM (21:00)"
        },
        <TimeChoice>{
            id: 44,
            value: "21:30",
            label: "9:30 PM (21:30)"
        },
        <TimeChoice>{
            id: 45,
            value: "22:00",
            label: "10:00 PM (22:00)"
        },
        <TimeChoice>{
            id: 46,
            value: "22:30",
            label: "10:30 PM (22:30)"
        },
        <TimeChoice>{
            id: 47,
            value: "23:00",
            label: "11:00 PM (23:00)"
        },
        <TimeChoice>{
            id: 48,
            value: "23:30",
            label: "11:30 PM (23:30)"
        },
        


    ];
    public propagateChange:any;

    _value = 0;

    control:NgControl;

    ngOnInit() {
        this.control = this.injector.get(NgControl);
        if(this.start == null && this.end == null){
            this.options = this.allOptions;
        }else{
            if(this.start == null) this.start = 0;
            if(this.end == null) this.end = 24;
            this.options = [];
            for( let opt of this.allOptions){
                let hours = opt.value.split(":");
                let hour:number = +hours[0];
                if( hour >= this.start && hour <= this.end) this.options.push(opt);
            }
        }
    }

    get timeValue() {
      return this._value;
    }
    set timeValue(val) {
        this._value = val;     
        this.propagateChange(val);
    }


    constructor(
        public injector: Injector
    ){
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

class TimeChoice{
    id:number;
    value: string;
    label:string;
}