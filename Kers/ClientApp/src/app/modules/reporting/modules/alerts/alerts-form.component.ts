import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, Validators } from '@angular/forms';
import { IAngularMyDpOptions } from 'angular-mydatepicker';
import { Role, RolesService } from '../admin/roles/roles.service';
import { Position, UsersService } from '../admin/users/users.service';
import { Alert, AlertRoute, AlertTypes, AlertType } from './Alert';
import { AlertsService } from './alerts.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'alerts-form',
  templateUrl: './alerts-form.component.html',
  styles: [
  ]
})
export class AlertsFormComponent implements OnInit {
  alertForm:any;
  date = new Date();
  endDate = new Date();
  roles: Role[];
  positions: Position[];
  errorMessage = "";
  alerts = AlertTypes;
  routes:Observable<AlertRoute[]>;
  loading = false;

  @Input() alert:Alert;

  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<Alert>();

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
    private fb: FormBuilder,
    private service:AlertsService,
    private rolesService: RolesService,
    private usersService: UsersService 
  ) {
    this.routes = service.routes();
    this.date = new Date( this.date.getFullYear(), this.date.getMonth(), this.date.getDate() );
    this.endDate = new Date( this.endDate.getFullYear(), this.endDate.getMonth(), this.endDate.getDate() + 5 );
    const urlRegex = /^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$/;
    this.alertForm = this.fb.group(
      {
          start:[{
              isRange: false, singleDate: {jsDate: this.date}
            }, Validators.required],
          end:[{
              isRange: false, singleDate: {jsDate: this.endDate}
            }, Validators.required],
          message: ["", Validators.required],
          alertType: ["", Validators.required],
          urlRoute: ["", Validators.required],
          moreInfoUrl: ["",Validators.pattern(urlRegex)],
          active: true,
          zEmpRoleTypeId: "",
          employeePositionId: [''],
          isContyStaff: 0
      }
    );


   }

  ngOnInit(): void {
    this.rolesService.listRoles().subscribe(
      res => {
          this.roles = <Role[]>res;
      },
            error => this.errorMessage = <any> error
      );
      this.usersService.positions().subscribe(
            res => {
                this.positions = <Position[]>res;
            },
            error => this.errorMessage = <any> error
      );
      if(this.alert){
        
        this.alertForm.patchValue(this.alert);
        this.alertForm.patchValue({
          start:{
            isRange: false, singleDate: {jsDate: new Date(this.alert.start)}
          },
          end:{
            isRange: false, singleDate: {jsDate: new Date(this.alert.end)}
          }
        })
      }
  }

  onDateChanged(event){

  }

  onSubmit(){
    this.loading = true;
    var alrt = this.alertForm.value as Alert;
    alrt.start = this.alertForm.value.start.singleDate.jsDate;
    alrt.end = this.alertForm.value.end.singleDate.jsDate;
    if(this.alert){
      this.service.updateAlert( this.alert.id, alrt ).subscribe(
        res => {
          this.onFormSubmit.emit(this.alert);
          this.loading = false;
        }
      )
      
    }else{
      this.service.addAlert(alrt).subscribe(
        res => {
          this.onFormSubmit.emit(res);
          this.loading = false;
        }
      )

    }
    

  }
  onCancel(){
    this.onFormCancel.emit();
  }

}

export const alertsValidator = (control: AbstractControl): {[key: string]: boolean} => {
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

