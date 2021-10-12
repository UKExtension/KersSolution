import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import {Location} from '@angular/common';
import { Servicelog } from '../servicelog/servicelog.service';
import  { SignupService } from './signup.service'

@Component({
  selector: 'signup',
  template: `
  
  
  <div class="signup-form-overlay">

  <div class="border">
    <div class="text-right" style="margin: 5px 0 15px;">
      <button class="btn btn-info btn-xs" (click)="closeOverlay()"><i class="fa fa-remove"></i></button>
    </div>
    <div class="row">
      <div class="col-md-4 col-lg-4 col-sm-6 col-xs-8 col-xs-offset-2 col-sm-offset-3 col-md-offset-0" style="padding-bottom: 35px;">
        <img src="{{logo}}" width="100%" style="margin: 5px"/>
      </div>
      <div class="col-lg-8 col-md-7 hidden-xs" style="padding: 2px 25px 25px;">
      The purpose of this questionnaire/form is to gather race, ethnicity, and gender information about persons who apply and participate in this Extension/USDA program. The information you provide will not be used when reviewing any application or when determining whether you are eligible to participate in this program. This is a voluntary questionnaire. You are not required to give this information, but we hope you will because the information you give will be used to improve the operation of this program, to help Extension/USDA design additional opportunities for program participation, and to monitor enforcement of laws that require equal access to this program for eligible persons. If you have previously provided your contact information, you only need to note your race, gender and ethnicity on the form. Your information will be kept private to the extent permitted by law. Thank you for your response.
      </div>
    </div>
    <div class="row">
      <div class="col-xs-offset-1">
        <div class="row">
          <div class="col-sm-4 col-xs-11 col-xs-offset-1 col-sm-offset-0">
            <strong>Meething:</strong><br>{{activity.title}}
          </div>
          <div class="col-sm-4 col-xs-11 col-xs-offset-1 col-sm-offset-0">
            <strong>Date:</strong><br>{{activity.activityDate | date:'fullDate'}}
          </div>
          <div class="col-sm-3 col-xs-11 col-xs-offset-1 col-sm-offset-0">
            <div [innerHtml]="'<strong>Purpose:</strong>' + activity.description"></div>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-xs-10 col-xs-offset-1">
        <signup-form [activity]="activity"></signup-form>
      </div>
    </div>
    <div class="row">
      <div class="col-xs-10 col-xs-offset-1">
      
      <br><br>
      
      
      (Disclosure of address, email, race, gender And/or ethnicity is voluntary)
        
        
        
        <br><br>

Educational programs of Kentucky Cooperative Extension serve all people regardless of economic or social starus and will not discriminate on the bases of race, color, ethnic origin, national origin, creed, religion, political belier, sex, sexual orientation, gender identity, gender expression, pregnancy, mrital status, genetic information, age, veteran status, or phisical or mental disability. University of Kentcucky, Kentucky State University, U.S. Department of Agriculture, and Kencucky Counties, Cooperating.





      </div>
    </div>


    



  
  </div>
  

</div>
  
  
  `,
  styles: [`
  .signup-form-overlay{
    background-color:rgba(220,239,230, 0.8);
    border: 3px solid rgba(120,139,130, 0.2);
    position: fixed;
    top:0;
    bottom:0;
    left:0;
    right:0;
    z-index: 100;
    padding: 10px;
  }
  .border{
    position: absolute;
    top: 2%;
    bottom: 2%;
    left: 4%;
    right: 4%;
    background-color:white;
    border: 2px solid #ccc;
    overflow: scroll;
  }
  `]
})
export class SignupComponent implements OnInit {
  @Input() activity:Servicelog;
  @Input() activityForm:any;
  @Output() onCancel = new EventEmitter<void>();

  logo:string;

  constructor(
    private location:Location,
    private service:SignupService
  ) { 
    this.logo = location.prepareExternalUrl('/assets/images/UKKSULogos.png');
  }

  ngOnInit(): void {
  }

  closeOverlay(){
    this.onCancel.emit();
  }

}
