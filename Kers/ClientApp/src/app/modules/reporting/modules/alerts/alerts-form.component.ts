import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { IAngularMyDpOptions } from 'angular-mydatepicker';

@Component({
  selector: 'alerts-form',
  templateUrl: './alerts-form.component.html',
  styles: [
  ]
})
export class AlertsFormComponent implements OnInit {
  alertForm:any;

  public myDatePickerOptions: IAngularMyDpOptions = {
    dateFormat: 'mm/dd/yyyy',
    satHighlight: true,
    firstDayOfWeek: 'su'
};
public myDatePickerOptionsEnd: IAngularMyDpOptions = {
      dateFormat: 'mm/dd/yyyy',
      satHighlight: true,
      firstDayOfWeek: 'su'
  };

defaultTime = "12:34:56.1000000 -04:00";



  constructor(
    private fb: FormBuilder
  ) {


    this.alertForm = this.fb.group(
      {
          message: ["", Validators.required]
      }
    );


   }

  ngOnInit(): void {
  }

  onDateChanged(event){

  }

  onSubmit(){

  }

}

export const trainingValidator = (control: AbstractControl): {[key: string]: boolean} => {
  let start = control.get('start');
  let end = control.get('end');
  var errors = {};
  var hasErrors = false;
  if( end.value != null && end.value.date != null){
    if(start.value != null){
      let startDate = new Date(start.value.date.year, start.value.date.month - 1, start.value.date.day);
      let endDate = new Date(end.value.date.year, end.value.date.month - 1, end.value.date.day);
      if( startDate.getTime() > endDate.getTime()){
        errors["endDate"] = true ;
        hasErrors = true;
      }
    }    
  }
  if(hasErrors){
    return errors;
  }
  return null;
}

