import { Component, Input, forwardRef, OnInit, Output, EventEmitter } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import {Location} from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'image-picker',
  template: `
            <img [froalaEditor]="options" [(froalaModel)]="imgObj" alt="Profile Image" class="col-xs-5 img-circle">
  `,
    providers:[  { 
                    provide: NG_VALUE_ACCESSOR,
                    useExisting: forwardRef(() => ImageUploadComponent),
                    multi: true
                  } 
                ]
})
export class ImageUploadComponent implements ControlValueAccessor, OnInit {

  @Input('userId') userId = 0;
  @Input('defaultImageName') defaultImage = "user.png";

  @Input() _value = 0;

  @Output() onUploaded = new EventEmitter<number>();
  @Output() onDeleted = new EventEmitter<number>();

  get imageId() {
      return this._value;
  }
  set imageId(val) {
      this._value = val;    
      this.propagateChange(val);
  }
  imageName;

  options = {};

  public propagateChange: any;

  public imgObj: Object = {
        src: this.location.prepareExternalUrl('/assets/images/user.png')
  };


  constructor(
        private location: Location,
        private http:HttpClient  

  ){
    
    this.propagateChange = () => {};
  }

  ngOnInit(){
      this.imgObj['src'] = this.location.prepareExternalUrl('/assets/images/'+this.defaultImage);
      var thisObject = this;
      this.options = { 
            imageUploadParams: { profileId: this.userId },
            imageUploadURL: this.location.prepareExternalUrl('/FroalaApi/UploadImage'),
            imageEditButtons: ['imageReplace', 'imageRemove'],
            events: {
                'froalaEditor.image.uploaded':function (e, editor, response){
                    var o = <ImageResponse>JSON.parse(response);
                    thisObject.imageId = o.imageId;
                },
                'froalaEditor.image.beforeRemove':function (e, editor, $img){
                    
                    editor.image.insert(thisObject.location.prepareExternalUrl('/assets/images/'+thisObject.defaultImage), false, null, editor.image.get());
                    thisObject.imageId = null;
                    return false;
                },
                'froalaEditor.image.error':function (e, editor, error, response){
                    console.log(error);
                    console.log(response);
                }

            }
        }
  }

  writeValue(value: any) {
      
      if (value !== "") {
        if(value != 0 ){
            this.http.get(this.location.prepareExternalUrl('/Image/id/'+value))
            .subscribe(
                res => {
                    var respns = res;
                    this.imageName = (respns as any).filename;
                    this.imgObj = {
                        src: this.location.prepareExternalUrl("/image/" + this.imageName)
                    };
                }
            );
        }
        
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