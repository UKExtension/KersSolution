import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { PlanningUnit } from '../../user/user.service';
import { CountyService } from '../../county/county.service';
import { FormBuilder, Validators } from '@angular/forms';
import { PlanningunitService } from '../planningunit.service';

@Component({
  selector: 'planning-unit-admin-form',
  templateUrl: './planning-unit-admin-form.component.html',
  styleUrls: ['./planning-unit-admin-form.component.css']
})
export class PlanningUnitAdminFormComponent implements OnInit {
  @Input() county:PlanningUnit;
  countyForm;
  options:object;
  loading = true;
  errorMessage:string;

  @Output() onFormCancel = new EventEmitter<void>();
  @Output() onFormSubmit = new EventEmitter<PlanningUnit>();

  constructor(
    private fb: FormBuilder,
    private service:PlanningunitService
  ) { 

    this.countyForm = fb.group(
      {
      
          name: ["", Validators.required],
          fullName: [""],
          description: [""],
          address: [""],
          city: [""],
          zip: [""],
          phone: [""],
          webSite: [""],
          email: [""],
          code: [""],
          geoFeature: [""],
          fIPSCode: [0],
          reportsExtension: [true],
          population: [0]
    });
    this.options = { 
      placeholderText: 'Your Description Here!',
      toolbarButtons: ['undo', 'redo' , 'bold', 'italic', 'underline', 'paragraphFormat', '|', 'formatUL', 'formatOL'],
      toolbarButtonsMD: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
      toolbarButtonsSM: ['undo', 'redo', 'bold', 'italic', 'underline', 'paragraphFormat'],
      toolbarButtonsXS: ['undo', 'redo', 'bold', 'italic'],
      quickInsertButtons: ['ul', 'ol', 'hr'],    
    }

  }

  ngOnInit() {
    this.countyForm.patchValue(this.county);
    this.loading = false;
  }
  onSubmit(){
    this.loading = true;
    this.service.update( this.county.id, this.countyForm.value).subscribe(
      res => {
        console.log(res);
        this.loading = false;
        this.onFormSubmit.emit(res);
      }
    );
    
  }
  onCancel(){
    this.onFormCancel.emit();
  }

}
