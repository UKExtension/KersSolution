import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Vehicle, VehicleService } from './vehicle.service';
import { FormBuilder, Validators } from '@angular/forms';
import { IMyDpOptions } from 'mydatepicker';
import { PlanningUnit } from '../../plansofwork/plansofwork.service';
import { User, UserService } from '../../user/user.service';

@Component({
  selector: 'vehicle-form',
  templateUrl: 'vehicle-form.component.html',
  styles: []
})
export class VehicleFormComponent implements OnInit {

  @Input() vehicle:Vehicle = null;
  @Input() county:PlanningUnit;
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<Vehicle>();
  vehicleForm;
  user:User;
  loading = false;
  errorMessage:string;
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
      private userService:UserService,
      private service:VehicleService,
  ) { 

    this.vehicleForm = fb.group(
            {
              uploadImageId: '',
              make: ['', Validators.required],
              model: ['', Validators.required],
              year: [''],
              licenseTag: ['', Validators.required],
              color: [''],
              odometer: [''],
              enabled: true,
              comments: [''],
              datePurchased: [{}],
              dateDispossed: [{}],
            }
                          );

  }

  ngOnInit() {
    if(this.vehicle != null){
      this.vehicleForm.patchValue(this.vehicle);
      if(this.vehicle.datePurchased != null){
        var dp = new Date(this.vehicle.datePurchased);
        this.vehicleForm.patchValue({datePurchased: {
          date: {
              year: dp.getFullYear(),
              month: dp.getMonth() + 1,
              day: dp.getDate()}
          }});
      }

      if(this.vehicle.dateDispossed != null){
        var dd = new Date(this.vehicle.dateDispossed);
        this.vehicleForm.patchValue({dateDispossed: {
          date: {
              year: dd.getFullYear(),
              month: dd.getMonth() + 1,
              day: dd.getDate()}
          }});
      }
    }
    this.userService.current().subscribe(
      res => this.user = res
    )
  }
  onSubmit(){
    this.loading = true;
    var val = this.vehicleForm.value;
    if(this.vehicleForm.value.datePurchased!= null && this.vehicleForm.value.datePurchased.date != null){
      var purchasedDateValue = this.vehicleForm.value.datePurchased.date;
      val.datePurchased = new Date(Date.UTC(purchasedDateValue.year, purchasedDateValue.month - 1, purchasedDateValue.day, 8, 5, 12));;
    }else{
      val.datePurchased = null;
    }
    if(this.vehicleForm.value.dateDispossed!= null && this.vehicleForm.value.dateDispossed.date != null){
      var disposedDateValue = this.vehicleForm.value.dateDispossed.date;
      val.dateDispossed = new Date(Date.UTC(disposedDateValue.year, disposedDateValue.month - 1, disposedDateValue.day, 8, 5, 12));
    }else{
      val.dateDispossed = null;
    }

    if(this.vehicle == null){
      val.planningUnitId = this.county.id;
      this.service.add(val).subscribe(
          res => {
              this.loading = false;
              this.onFormSubmit.emit(<Vehicle>res);
          },
          err => this.errorMessage = <any>err
      );
    }else{
        this.service.update(this.vehicle.id, val).subscribe(
            res => {
                this.loading = false;
                this.onFormSubmit.emit(<Vehicle>res);
            },
            err => this.errorMessage = <any>err
        );
    }
    
        


  }
  onCancel(){
    this.onFormCancel.emit();
  }

}
