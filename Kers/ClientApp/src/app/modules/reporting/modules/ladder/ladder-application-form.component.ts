import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import {Location} from '@angular/common';
import { IAngularMyDpOptions, IMyDateModel } from 'angular-mydatepicker';
import { FormBuilder, Validators, FormArray, FormGroup, FormControl } from '@angular/forms';
import { LadderService, FileUploadResult } from './ladder.service';
import { LadderLevel, LadderEducationLevel, LadderPerformanceRating, LadderApplication, LadderImage, UploadImage } from './ladder';
import { Observable } from 'rxjs';
import { TrainingService } from '../training/training.service';
import { UserService, User } from '../user/user.service';
import { Router } from '@angular/router';
import { FiscalYear, FiscalyearService } from '../admin/fiscalyear/fiscalyear.service';

@Component({
  selector: 'ladder-application-form',
  templateUrl: './ladder-application-form.component.html',
  styles: []
})
export class LadderApplicationFormComponent implements OnInit {

    @Input() application:LadderApplication;

    
    private currentUser:User;

    messages: string[] = [];

    isItaDraft:boolean = false;
    loading:boolean = true;
    today:Date;

    get ratings() {
      return this.ladderForm.get('ratings') as FormArray;
    }

    get formImages() {
      return this.ladderForm.get('images') as FormArray;
    }
  
    ladderForm:any;
    levels:Observable<LadderLevel[]>;
    educationLevels:Observable<LadderEducationLevel[]>;
    trainingDetails = false;

    images:LadderImage[] = [];
    imageIsUploading = false;

    lastPromotionDate:Date;
    hoursAttended:Observable<number>;

    firstOfTheYear:Date;

    fileToUpload: File = null;

    options = { 
        placeholderText: 'Your Description Here!',
        toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL'],
        toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
        toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
        toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
        quickInsertButtons: ['ul', 'ol', 'hr'],    
      };
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
      

  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<LadderApplication>();
  @Output() onDraftSaved = new EventEmitter<LadderApplication>();

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    private service:LadderService,
    private trainingService:TrainingService,
    private fiscalYearService:FiscalyearService,
    private location: Location,
    private router: Router
  ) {
        this.userService.current().subscribe(
          res => this.currentUser = res
        );

        
        var threeYEarsAgo = new Date(new Date().setFullYear(new Date().getFullYear() - 3));
        var fiveYearsAgo = new Date(new Date().setFullYear(new Date().getFullYear() - 5));
        this.ladderForm = this.fb.group(
        {
          ladderLevelId: ["", Validators.required],
          lastPromotion: [{
                      isRange: false, singleDate: {jsDate: threeYEarsAgo}
              }, Validators.required],
          startDate: [{
                      isRange: false, singleDate: {jsDate: fiveYearsAgo}
                }, Validators.required],
          ladderEducationLevelId:["", Validators.required],
          track:"0",
          programOfStudy: [""],
          evidence: [""],
          numberOfYears: ["",[Validators.required, Validators.pattern("^[0-9]*$")]],
          ratings: this.fb.array([]),
          images: this.fb.array([])
    },);
    this.today = new Date();
    this.myDatePickerOptions.disableSince = {year: this.today.getFullYear(), month: this.today.getMonth() + 1, day: this.today.getDate()};
    this.lastPromotionDate = new Date(this.today.getFullYear() -3, 6, 1);
    //this.firstOfTheYear = new Date(this.today.getFullYear(), 0, 1);
   }

  ngOnInit() {
    this.levels = this.service.levels();
    this.educationLevels = this.service.educationLevels();
    if( this.application != null){
      this.lastPromotionDate = this.application.lastPromotion;
      this.ladderForm.patchValue(this.application);
      var lastPromotion = new Date( this.application.lastPromotion);
      var startDate = new Date(this.application.startDate);
      this.ladderForm.patchValue({
        lastPromotion: {
          isRange: false, singleDate: {jsDate: lastPromotion}
          },
          startDate: {
            isRange: false, singleDate: {jsDate: startDate}
          }
        
      }); 
      this.lastPromotionDate = new Date(this.application.lastPromotion);
      for( let rating of this.application.ratings){
        this.addRating( rating.year, rating.ratting );
      }
      for( let img of this.application.images){
        this.addImage( img.uploadImageId, img.uploadImage.name, img.description);
      }
      this.fiscalYearService.forDate(new Date(this.application.created)).subscribe(
        res => {
          var end = new Date(res.end);
          this.firstOfTheYear = new Date(end.getFullYear(), 0, 1);
          this.updateHours();
          this.loading = false;
        }
      );
    }else{
      this.addRating();
      this.fiscalYearService.current().subscribe(
        res => {
          var end = new Date(res.end);
          this.firstOfTheYear = new Date(end.getFullYear(), 0, 1);
          this.updateHours();
          this.loading = false;
        }
      );
    }
  }
  updateHours(){
    this.hoursAttended = this.trainingService.hoursByUser(0,this.lastPromotionDate, this.firstOfTheYear);
  }

  addRating(year:string = '', rating:string = '') {
    const group = new FormGroup({
      year: new FormControl(year, [Validators.required, 
        Validators.pattern('^\\d*$')]),
      ratting: new FormControl(rating, [Validators.required, 
        Validators.pattern('^\\d*$')])
    });
    this.ratings.push(group);
  }
  removeRating(i:number){
    this.ratings.removeAt(i);
  }
  handleFileInput(files: FileList) {
    this.fileToUpload = files.item(0);

    var uploaded = this.service.postFile(this.fileToUpload, this.currentUser.id);
    this.imageIsUploading = true;
    uploaded.subscribe(
          res => {
            var reslt = <FileUploadResult> res;
            if(reslt.success){
              const group = new FormGroup({
                description: new FormControl(''),
                imageId: new FormControl(''),
                imageName: new FormControl('')
              });
              group.patchValue({imageId:reslt.imageId, imageName:reslt.fileName});
              this.formImages.push(group);
              var img = new LadderImage;
              img.uploadImageId = reslt.imageId;
              img.uploadImage = <UploadImage>{};
              img.uploadImageId = reslt.imageId;
              img.uploadImage.name = reslt.fileName;
              this.images.push(img);
            }else{
              this.addMessage(reslt.message);
            }
            this.imageIsUploading = false;
          }
      );
  }

  addImage(imageId:number = 0, imageName = "", imageDescription = ""){
    const group = new FormGroup({
      description: new FormControl(imageDescription),
      imageId: new FormControl(imageId),
      imageName: new FormControl(imageName)
    });
    this.formImages.push(group);
    var img = new LadderImage;
    img.uploadImageId = imageId;
    img.uploadImage = <UploadImage>{};
    img.uploadImage.name = imageName;
    this.images.push(img);
  }

  getImageSrc(filename:string){
    return this.location.prepareExternalUrl("/image/" + filename);
  }

  deleteImageClick(imageId:number, index:number){
    if( this.application == null ){
      this.service.deleteImage(imageId).subscribe(
        res => {
          this.formImages.removeAt(index);
          this.images.splice(index, 1);
        }
      );
    }else{
      this.formImages.removeAt(index);
      this.images.splice(index, 1);
    }
  }

  addMessage(message: string) {
    this.messages.push(message);
    setTimeout(() => { 
      let index: number = this.messages.indexOf(message);
        if (index !== -1) {
            this.messages.splice(index, 1);
        }
    }, 6000);
  }

  onDateChanged(event: IMyDateModel) {
    this.lastPromotionDate = event.singleDate.jsDate;
    this.updateHours();
    this.trainingDetails = false;
  }

  draftSaved(){
    this.isItaDraft = true;
  }

  onSubmit(){
    this.loading = true;
    var formValue = this.ladderForm.value;
    var application = <LadderApplication> Object.assign(formValue);
    application.lastPromotion = this.ladderForm.value.lastPromotion.singleDate.jsDate;
    application.startDate = this.ladderForm.value.startDate.singleDate.jsDate;
    var ratingsSubmitted:LadderPerformanceRating[] = [];
    var i = 1;
    for( var rating of formValue.ratings){
      var r = <LadderPerformanceRating>{
        year:rating['year'],
        ratting: rating['ratting'],
        order: i
      };
      i++;
      ratingsSubmitted.push(r);
    }
    var ladderImages:LadderImage[] = [];
    for( var img of application.images){
      var im = <LadderImage>{
        uploadImageId:img['imageId'],
        description: img['description'],
        created: new Date()
      }
      ladderImages.push(im);
    }
    application.ratings = ratingsSubmitted;
    application.images = ladderImages;
    application.kersUserId = this.currentUser.id;
    application.draft = this.isItaDraft;
    if( this.application != null){
      this.service.update(this.application.id, application).subscribe(
        res => {
          if(this.isItaDraft){
            this.onDraftSaved.emit(res);
          }else{
            this.onFormSubmit.emit(res);
          }
          this.isItaDraft = false;
          this.loading = false;
  
        }
      )   
    }else{
      this.service.add(application).subscribe(
        res => {
          if(this.isItaDraft){
            this.onDraftSaved.emit(res);
          }else{
            this.onFormSubmit.emit(res);
          }
          this.isItaDraft = false;
          this.loading = false;
  
        }
      )   
    }
  }
  onCancel(){
    if( this.application == null && this.images.length > 0){
      this.loading = true;
      // Delete at least one of the uploaded images that are not needed
      // Figure out how to delete multiple in order not to waste disk space
      this.service.deleteImage( this.images[0].uploadImageId).subscribe(
        _ => {
          this.loading = false;
          this.onFormCancel.emit();
        }
      )
    }else{
      this.onFormCancel.emit();
    }   
  }

}

