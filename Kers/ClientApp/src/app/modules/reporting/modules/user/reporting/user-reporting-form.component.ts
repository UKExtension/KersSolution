import { Component,OnInit, Input, EventEmitter, Output } from '@angular/core';
import { map, take, debounceTime } from 'rxjs/operators';
import {    UserService,
            ExtensionPosition, 
            User,
            ReportingProfile,
            PersonalProfile,
            PlanningUnit,
            GeneralLocation,
            UserSpecialty,
            Specialty, 
            Institution
        } from '../user.service';
import { FormBuilder, Validators, AbstractControl, FormControl } from '@angular/forms';
import {ReportingService} from '../../../components/reporting/reporting.service';
import { Observable } from "rxjs/Observable";
import {Router} from '@angular/router';
import {Location} from '@angular/common';
import { AuthHttp } from '../../../../authentication/auth.http';

@Component({
    selector: 'user-reporting-form',
    templateUrl: 'user-reporting-form.component.html',
    styles: [`
    .toggle{
        padding-top: 0px important!;
    }
    `]
})
export class UserReportingFormComponent implements OnInit { 

    
    @Input() userObservable:Observable<User> = null;
    @Input() adminEdit:boolean = false;




    @Output() onFormCancel = new EventEmitter<void>();
    @Output() onFormSubmit = new EventEmitter<User>();

    reportingForm = null;
    public options: Object;

    public user:User;

    public positions: Observable<ExtensionPosition[]>;
    public specialties: Observable<Specialty[]>;
    public specialtiesOptions:Array<any>;
    public locations: Observable<GeneralLocation[]>
    public units: Observable<PlanningUnit[]>
    public institutions: Observable<Institution[]>
    public loading = false;

    errorMessage: string;


    constructor( 
        private userService: UserService,
        private fb: FormBuilder, 
        private reportingService: ReportingService,
        private router: Router,
        private http:AuthHttp,
        private location:Location
    )   
    {



       

    }

    ngOnInit(){

        this.loading = true;

        if(this.adminEdit){
            var isDisabled = true;
            if(this.userObservable == null){
                isDisabled = false;
            }

            this.reportingForm = this.fb.group(
                {
                extensionPositionId: ['', Validators.required],
                specialties: '',
                rprtngProfile: this.fb.group(
                    {
                        planningUnitId: ['', Validators.required],
                        generalLocationId: ['', Validators.required],
                        email: ['', Validators.required],
                        emailAlias: '',
                        enabled: false,
                        personId: [{value: '', disabled: isDisabled}, Validators.required, CustomValidator.personId(this.userService)],
                        linkBlueId: [{value: '', disabled: isDisabled}, Validators.required, CustomValidator.linkBlueId(this.userService)],
                        institutionId: ['', Validators.required],
                        name: ['', this.doesItContainComma],
                    }
                )
                }
            );
        }else{
            this.reportingForm = this.fb.group(
                {
                extensionPositionId: ['', Validators.required],
                specialties: '',
                rprtngProfile: this.fb.group(
                    {
                        planningUnitId: ['', Validators.required],
                        generalLocationId: ['', Validators.required],
                        email: ['', Validators.required],
                        emailAlias: ''
                    }
                )
                }
            );
        }



        //Positions
        this.positions = this.userService.extensionPositions();
        this.positions.subscribe(
            res => {<ExtensionPosition[]> res},
            error => <any> this.errorMessage
        );
        //Locations
        this.locations = this.userService.locations();
        this.locations.subscribe(
            res => {<GeneralLocation[]> res},
            error => <any> this.errorMessage
        );
        //Planning Units
        this.units = this.userService.units();
        this.units.subscribe(
            res => {
                //User
                if(this.userObservable != null){
                    this.userObservable.subscribe(
                        res=>{
                            this.user = <User>res;
                            var sp = [];
                            this.user.specialties.forEach(function(element){
                                sp.push(element.specialty.id);
                            });
                            this.reportingForm.patchValue(this.user);
                            this.reportingForm.patchValue({specialties:sp});
                            this.loading = false;
                        },
                        error => <any> this.errorMessage
                    );
                }else{
                    this.loading = false;
                }



                return <PlanningUnit[]> res
            },
            error => <any> this.errorMessage
        );
        this.institutions = this.userService.instititutions();
        //Specialties
        this.specialties = this.userService.specialties();
        this.specialties.subscribe(
            res => {
                var spclt = <Specialty[]> res;
                var sp = Array<any>();
                spclt.forEach(function(element){
                    sp.push(
                        {value: element.id, label: element.name}
                    );
                });
                this.specialtiesOptions = sp;
                return spclt;
            },
            error => <any> this.errorMessage
        );
        

    }

    onSubmit(){
        



        this.loading = true;
        var val = this.reportingForm.value;
        
        
        var spcltys = this.reportingForm.value.specialties;
        var realSp = [];
        
        if(this.userObservable == null){
            if(spcltys == ""){
                val.specialties = [];
            }else{
                spcltys.forEach(function(element){
                    var s = { specialtyId: element };
                    realSp.push(s);
                });
                val.specialties = realSp; 
            }
            
            this.userService.add(<User>val).subscribe(
                res => {
                    this.loading = false;
                    this.onFormSubmit.emit(<User> res);
                }
            );
            this.loading = false;
        }else{
            if(this.user){
                
                var usr = this.user;
        
                
                spcltys.forEach(function(element){
                    var s = { kersUserId: usr.id, specialtyId: element};
                    realSp.push(s);
                });
                
                val.specialties = realSp;
                if(this.user.id != 0){
                    this.userService.update(this.user.id, <User> val  ).subscribe(
                        res => {
                            this.loading = false;
                            this.onFormSubmit.emit(<User> res);
                        },
                        error => this.errorMessage = <any>error
                    )
                }else{
                    var u = <User> val;
                    u.rprtngProfile.linkBlueId = this.user.rprtngProfile.linkBlueId;
                    u.rprtngProfile.personId = this.user.rprtngProfile.personId;
                    u.rprtngProfile.name = this.user.rprtngProfile.name;
                    u.personalProfile = this.user.personalProfile;
                    this.userService.add( u ).subscribe(
                        res => {
                            this.loading = false;
                            this.onFormSubmit.emit(<User> res);
                        },
                        error => this.errorMessage = <any>error
                    )
                }
            }
        }

    }

    OnCancel(){
        this.onFormCancel.emit();
    }


    doesItContainComma(control:FormControl){
        if(control.value.match(/,/)){
            return null;
        }
        return {"notComa":true};
    }
    
}

export class CustomValidator {
    static linkBlueId(userService: UserService) {
      return (control: AbstractControl) => {
        
        return userService.linkBlueExists(control.value).pipe(
            debounceTime(500)
        );
  
      }
    }
    static personId(userService: UserService) {
        return (control: AbstractControl) => {
          
          return userService.personIdExists(control.value).pipe(
              debounceTime(500)
          );
    
        }
      }
  }