import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { FormBuilder, Validators, AbstractControl} from '@angular/forms';
import {Location} from '@angular/common';
import { CountyEvent, CountyEventService, CountyEventProgramCategory, CountyEventPlanningUnit, CountyEventWithTime } from './county-event.service';
import { IMyDpOptions, IMyDateModel } from 'mydatepicker';
import { ProgramCategory, ProgramsService } from '../../admin/programs/programs.service';
import { PlanningunitService } from '../../planningunit/planningunit.service';
import { UserService, PlanningUnit } from '../../user/user.service';
import { ExtensionEventLocation } from '../extension-event';
import { ExtensionEventLocationConnection } from '../location/location.service';

@Component({
  selector: 'county-event-form',
  templateUrl: './county-event-form.component.html',
  styleUrls: ['./county-event-form.component.scss']
})
export class CountyEventFormComponent implements OnInit {
  
  @Input() countyEvent:CountyEventWithTime;
  county:PlanningUnit;
  countyId: number;
  date = new Date();
  countyEventForm:any;
  hasEndDate = false;
  easternTimezone = true;
  programCategories: ProgramCategory[];
  planningUnits: PlanningUnit[];
  programCategoriesOptions = Array<any>();
  planningUnitsOptions = Array<any>();
  multycounty = false;
  selectedLocation:ExtensionEventLocation;
  locationBrowser = true;
  options:object;
  
  loading = true;
  public myDatePickerOptions: IMyDpOptions = {
          dateFormat: 'mm/dd/yyyy',
          showTodayBtn: false,
          satHighlight: true,
          firstDayOfWeek: 'su',
          showClearDateBtn: false
      };
  public myDatePickerOptionsEnd: IMyDpOptions = {
            dateFormat: 'mm/dd/yyyy',
            showTodayBtn: false,
            satHighlight: true,
            firstDayOfWeek: 'su',
            showClearDateBtn: true
        };
    
  defaultTime = "12:34:56.1000000 -04:00";

    
  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<CountyEvent>();

  constructor(
    private fb: FormBuilder,
    private service: CountyEventService,
    private programsService: ProgramsService,
    private location: Location,
    private planningUnitService: PlanningunitService,
    private userService:UserService
  ) {
    this.date = new Date( this.date.getFullYear(), this.date.getMonth() + 3, this.date.getDate() );
    this.countyEventForm = this.fb.group(
        {
          start: [{
            date: {
                year: this.date.getFullYear(),
                month: this.date.getMonth() + 1,
                day: this.date.getDate()}
            }, Validators.required],
          starttime: [""],
          end: [null],
          endtime: "",
          etimezone:true,
          subject:["", Validators.required],
          body:"",
          webLink:"",
          programCategories:'',
          units: '',
          multycounty: false,
          location:this.fb.group(
            {
              address: this.fb.group(
                {
                  building: [""],
                  street: [""],
                  city: [""],
                  postalCode: ""
                }
              )
            }
          ),
          isCancelled: false
            
        }, { validator: trainingValidator }
      );
  }

  ngOnInit() {
    if(this.countyEvent){
      this.countyEventForm.patchValue(this.countyEvent);
      var start = new Date( this.countyEvent.start);
      this.countyEventForm.patchValue({
        start: {
          date:{
            year: start.getFullYear(),
            month: start.getMonth() + 1,
            day: start.getDate()
          }
        },
        etimezone: this.countyEvent.etimezone,
        starttime: this.countyEvent.starttime
      });
      if( this.countyEvent.end ){
        var end = new Date(this.countyEvent.end);
        this.countyEventForm.patchValue({
          end:{
            date:{
              year: end.getFullYear(),
              month: end.getMonth() + 1,
              day: end.getDate()
            }
          }
        })
      }
      if( this.countyEvent.location != null){
        this.locationBrowser = false;
        this.selectedLocation = this.countyEvent.location;
      }
      

    }
    this.userService.current().subscribe(
      res =>{

        var thisObject = this;
        this.options = { 
          placeholderText: 'Your Description Here!',
          toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL', 'insertImage'],
          toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'insertImage'],
          toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'insertImage'],
          toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
          quickInsertButtons: ['ul', 'ol', 'hr'],
          imageUploadParams: { profileId: res.id },
                                imageUploadURL: this.location.prepareExternalUrl('/FroalaApi/UploadImage'),  
                                events: {
                                    'froalaEditor.image.uploaded':function (e, editor, response){
                                        var o = <ImageResponse>JSON.parse(response);
                                        //var im = <ActivityImage>{uploadImageId: o.imageId}
                                        //thisObject.activityImages.push(im);
                                    }    
                                  }
        };



        this.county = res.rprtngProfile.planningUnit;
        if( !this.countyEvent && (this.county.timeZoneId == "Central Standard Time" ||this.county.timeZoneId == "America/Chicago") ){
          this.countyEventForm.patchValue({etimezone:false});
          this.easternTimezone = false;
        }
        this.countyId = res.rprtngProfile.planningUnitId;
        var cntId = res.rprtngProfile.planningUnitId;
        this.planningUnitService.counties().subscribe(
          res =>{
            this.planningUnits = res;
            var optns = Array<any>();
            this.planningUnits.forEach(function(element){
              if(element.id != cntId){
                optns.push(
                  {value: element.id, label: element.name}
                );
              }
              
            });
            this.planningUnitsOptions = optns;
            if(this.countyEvent != null && this.countyEvent.units != undefined && this.countyEvent.units.length > 1 ){
              this.multycounty = true;
              var cnts = [];
              for( var cnt of this.countyEvent.units ){
                if( cnt.planningUnitId != cntId){
                  cnts.push(
                    cnt.planningUnitId
                  );
                }
              }
              this.countyEventForm.patchValue({units:cnts, multycounty:true})
            } 
            



          }
        );
      } 
    )
    this.programsService.categories().subscribe(
      res =>{
        this.programCategories = res;
        var optns = Array<any>();
        this.programCategories.forEach(function(element){
          optns.push(
                {value: element.id, label: element.name}
            );
        });
        this.programCategoriesOptions = optns;

        if(this.countyEvent != null && this.countyEvent.programCategories != undefined && this.countyEvent.programCategories.length > 0 ){
          var ctgrs = [];
          for( var cnt of this.countyEvent.programCategories ){
              ctgrs.push(
                cnt.programCategoryId
              );
          }
          this.countyEventForm.patchValue({programCategories:ctgrs})
        } 


      } 
    );
  }

  locationSelected(event:ExtensionEventLocationConnection){
    this.selectedLocation = event.extensionEventLocation;
    this.locationBrowser = false;
  }

  editLocation(){
    this.locationBrowser = true;
  }

  onDateChanged(event: IMyDateModel) {
  }
  onMultyChecked(){
    this.multycounty = !this.multycounty;
  }
  isEastern(isIt:boolean){
    this.easternTimezone = isIt;
  }

  onSubmit(){
    this.loading = true;
    var result = <CountyEventWithTime> this.countyEventForm.value;

    result.start = new Date(this.countyEventForm.value.start.date.year, this.countyEventForm.value.start.date.month - 1, this.countyEventForm.value.start.date.day);
    if( this.countyEventForm.value.end != null && this.countyEventForm.value.end.date != null ){
      result.end = new Date(this.countyEventForm.value.end.date.year, this.countyEventForm.value.end.date.month - 1, this.countyEventForm.value.end.date.day);
    }else{
      result.end = null;
    }
    if(result.programCategories.length > 0){
      var cats = <CountyEventProgramCategory[]>[];
      for( let catId of this.countyEventForm.value.programCategories) cats.push( { programCategoryId: catId } as CountyEventProgramCategory);
      result.programCategories = cats;
    }


    var unts = <CountyEventPlanningUnit[]>[];
    unts.push({planningUnitId:this.countyId, isHost:true} as CountyEventPlanningUnit)
    if( this.countyEventForm.value.multycounty ){
      for( let unitId of this.countyEventForm.value.units){
        unts.push( { planningUnitId: unitId, isHost:false } as CountyEventPlanningUnit);
      }
    }
    result.units = unts;
    result.location = this.selectedLocation;
    
    
    if( this.countyEvent ){
      this.service.update( this.countyEvent.id, result).subscribe(
        res => {
          this.loading = false;
          this.onFormSubmit.emit(res);
        }
      )
    }else{
      this.service.add(result).subscribe(
        res => {
          this.loading = false;
          this.countyEventForm.reset();
          this.onFormSubmit.emit(res);
        }
      )
    }
    
    
  }
  onCancel(){
    this.onFormCancel.emit();
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

interface ImageResponse{

  link:string,
  imageId:number


};