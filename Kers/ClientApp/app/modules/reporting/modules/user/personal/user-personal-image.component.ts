import { Component, Input, forwardRef, OnInit } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import {UserService} from '../user.service';
import {Location} from '@angular/common';

@Component({
  selector: 'profile-image-picker',
  template: `
            <img [froalaEditor]="options" [(froalaModel)]="imgObj" alt="Profile Image" class="col-xs-5 img-circle">
  `,
    providers:[  { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => UserPersonalImageComponent),
                    multi: true
                  } 
                ]
})
export class UserPersonalImageComponent implements ControlValueAccessor, OnInit {

  @Input('userId') userId;

  @Input() _value = 0;

  get imageId() {
      return this._value;
  }
  set imageId(val) {
      this._value = val;     
      this.propagateChange(val);
  }

  options = {};

  public propagateChange: any;

  public imgObj: Object = {
        src: this.location.prepareExternalUrl('/dist/assets/images/user.png')
  };


  constructor(
        private userService: UserService,
        private location: Location  

  ){
    
    this.propagateChange = () => {};
  }

  ngOnInit(){
      var thisObject = this;
      this.options = { 
            imageUploadParams: { profileId: this.userId },
            imageUploadURL: this.location.prepareExternalUrl('/FroalaApi/UploadImage'),
            imageEditButtons: ['imageReplace', 'imageRemove'],
            events: {
                'froalaEditor.image.uploaded':function (e, editor, response){
                    var o = <ImageResponse>JSON.parse(response);
                    thisObject.imageId = o.imageId;
                }
            }
        }
  }

  writeValue(value: any) {
      if (value !== "") {
        this.userService.filenameForImageId(<number> value).subscribe(
            res => {
                var rsp = res.json();
                this.imgObj = {
                        src: this.location.prepareExternalUrl("/image/" + rsp.filename)
                };
            }
        );
        this.imageId = value;
      }
  }


  registerOnChange(fn) {
    this.propagateChange = fn;
  }

  registerOnTouched() {}

  
}

interface ImageResponse{

        link:string,
        imageId:number

    
};