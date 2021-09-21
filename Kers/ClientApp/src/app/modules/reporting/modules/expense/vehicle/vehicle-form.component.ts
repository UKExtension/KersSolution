import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Vehicle, VehicleService } from './vehicle.service';
import { FormBuilder, Validators } from '@angular/forms';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
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
  

    public myDatePickerOptions: IAngularMyDpOptions = {
      dateFormat: 'mm/dd/yyyy',
      satHighlight: true,
      firstDayOfWeek: 'su'
  };
  

  constructor(
      private fb: FormBuilder,
      private userService:UserService,
      private service:VehicleService,
  ) { 

    this.vehicleForm = fb.group(
            {
              uploadImageId: '',
              name: [''],
              make: ['', Validators.required],
              model: ['', Validators.required],
              year: [''],
              licenseTag: ['', Validators.required],
              purchasePrice: [''],
              color: [''],
              odometer: [''],
              endingOdometer: [''],
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
                    isRange: false, singleDate: {jsDate: dp}
                  }
                }
              );
      }

      if(this.vehicle.dateDispossed != null){
        var dd = new Date(this.vehicle.dateDispossed);
        this.vehicleForm.patchValue({dateDispossed: {
            isRange: false, singleDate: {jsDate: dd}
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
    if(this.vehicleForm.value.datePurchased != null && this.vehicleForm.value.datePurchased.singleDate != null){
      var purchasedDateValue = this.vehicleForm.value.datePurchased.singleDate.jsDate;
      val.datePurchased = new Date(Date.UTC(purchasedDateValue.getFullYear(), purchasedDateValue.getMonth(), purchasedDateValue.getDate(), 8, 5, 12));;
    }else{
      val.datePurchased = null;
    }
    if(this.vehicleForm.value.dateDispossed!= null && this.vehicleForm.value.dateDispossed.singleDate != null){
      var disposedDateValue = this.vehicleForm.value.dateDispossed.singleDate.jsDate;
      val.dateDispossed = new Date(Date.UTC(disposedDateValue.getFullYear(), disposedDateValue.getMonth(), disposedDateValue.getDate(), 8, 5, 12));
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
