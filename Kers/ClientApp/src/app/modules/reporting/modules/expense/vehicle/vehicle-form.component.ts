import { Component, OnInit, Input } from '@angular/core';
import { Vehicle, VehicleService } from './vehicle.service';
import { FormBuilder, Validators } from '@angular/forms';
import { IMyDpOptions } from 'mydatepicker';

@Component({
  selector: 'vehicle-form',
  templateUrl: 'vehicle-form.component.html',
  styles: []
})
export class VehicleFormComponent implements OnInit {

  @Input() expense:Vehicle = null;
  vehicleForm;
  private myDatePickerOptions: IMyDpOptions = {
    // other options...
        dateFormat: 'mm/dd/yyyy',
        showTodayBtn: false,
        satHighlight: true,
        firstDayOfWeek: 'su',
        allowDeselectDate: true,
        showClearDateBtn: true,
        editableDateField: true
    };

  constructor(
      private fb: FormBuilder,
      private service:VehicleService,
  ) { 

    this.vehicleForm = fb.group(
            {
              make: ['', Validators.required],
              model: ['', Validators.required],
              year: [''],
              licenseTag: ['', Validators.required],
              color: [''],
              enabled: true,
              comments: [''],
              datePurchesed: [{}],
              dateDisposed: [{}],
            }
                          );

  }

  ngOnInit() {
  }

}
