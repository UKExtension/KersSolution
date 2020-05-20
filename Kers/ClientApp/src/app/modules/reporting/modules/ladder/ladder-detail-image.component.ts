import { Component, OnInit, Input } from '@angular/core';
import { LadderImage } from './ladder';
import {Location} from '@angular/common';

@Component({
  selector: 'ladder-detail-image',
  template: `
    <div [ngClass]="zoomed ? 'col-xs-12' : 'col-xs-2'" >
      {{image.description}}<br>
      <img src="{{src}}" width="100%" (click)="zoomed = !zoomed" />
      
    </div>
  `,
  styles: []
})
export class LadderDetailImageComponent implements OnInit {
  @Input() image:LadderImage;
  src:string;
  zoomed=false;
  constructor(
    private location: Location
  ) { }

  ngOnInit() {
    this.src = this.location.prepareExternalUrl("/image/" + this.image.uploadImage.name);
  }
  

}
